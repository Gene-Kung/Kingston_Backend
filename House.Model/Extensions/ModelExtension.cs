using Dapper;
using House.Model.API;
using House.Model.Attributes;
using House.Model.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.Extensions
{
    public static class ModelExtension
    {
        public static T CopyProp<T>(this object source)
        {
            T target = Activator.CreateInstance<T>();
            var parentProperties = source.GetType().GetProperties();
            var childProperties = target.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        var value = parentProperty.GetValue(source);
                        if (value is string && (string.IsNullOrEmpty((string)value) || "NULL" == ((string)value).ToUpper()))
                            value = "";
                        childProperty.SetValue(target, value);
                        break;
                    }
                }
            }
            return target;
        }

        /// <summary>
        /// 型別不分是否為Nullable，例如:int 可映射至 int?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CopyPropIgnoreNullable<T>(this object source)
        {
            T target = Activator.CreateInstance<T>();
            var parentProperties = source.GetType().GetProperties();
            var childProperties = target.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name &&
                        (parentProperty.PropertyType == childProperty.PropertyType
                           || parentProperty.PropertyType == Nullable.GetUnderlyingType(childProperty.PropertyType)))
                    {
                        var value = parentProperty.GetValue(source);
                        childProperty.SetValue(target, value);
                        break;
                    }
                }
            }
            return target;
        }

        public static void CopyProp<T>(this object source, T target)
        {
            var parentProperties = source.GetType().GetProperties();
            var childProperties = target.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        var value = parentProperty.GetValue(source);
                        if (value is string && (string.IsNullOrEmpty((string)value) || "NULL" == ((string)value).ToUpper()))
                            value = "";
                        childProperty.SetValue(target, value);
                        break;
                    }
                }
            }
        }

        public static DynamicParameters ToSqlParamsList(this object obj)
        {
            //var dicParam = new Dictionary<string, object>();
            var parameters = new DynamicParameters();
            Type type = obj.GetType();
            foreach (var propertyInfo in type.GetProperties())
            {
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(obj);
                var dbType = propertyInfo.PropertyType.GetDbTypeFromPropertyType();
                parameters.Add($"@{name}", value, dbType, direction : ParameterDirection.Input);
            }
            return parameters;
        }

        public static DbType GetDbTypeFromPropertyType(this Type propertyType)
        {
            switch (Type.GetTypeCode(propertyType))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.DateTime:
                    return DbType.DateTime2;
                case TypeCode.String:
                    return DbType.String;
                // 添加其他类型的映射...

                default:
                    return DbType.Object;
            }
        }

        public static BaseQueryParams GenBaseQueryParams(this object obj)
        {
            var baseQueryParams = new BaseQueryParams();
            Type type = obj.GetType();

            var propertyInfos = type.GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(ConditionAttribute)));

            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetValue(obj);
                if (value == null ||
                    (propertyInfo.PropertyType == typeof(string) && string.IsNullOrEmpty(value.ToString())) || //string type
                    (propertyInfo.PropertyType == Nullable.GetUnderlyingType(typeof(string)) && string.IsNullOrEmpty(value.ToString()))) //string? type
                    continue;

                var dbType = propertyInfo.PropertyType.GetDbTypeFromPropertyType();

                var name = propertyInfo.Name;

                var paramAttribute = propertyInfo.GetCustomAttribute<ConditionAttribute>();

                switch (paramAttribute.Type) 
                {
                    case Enums.ParamAttributeEnum.Like:
                        baseQueryParams.QueryString.Add($"{name} like @{name}");
                        baseQueryParams.QueryParams.Add($"@{name}", $"%{value}%", dbType, direction: ParameterDirection.Input);
                        break;

                    case Enums.ParamAttributeEnum.Equal:
                        baseQueryParams.QueryString.Add($"{name} = @{name}");
                        baseQueryParams.QueryParams.Add($"@{name}", value, dbType, direction: ParameterDirection.Input);
                        break;

                    case Enums.ParamAttributeEnum.Interval:
                        if (paramAttribute.IsStart)
                        {
                            baseQueryParams.QueryString.Add($"{paramAttribute.TargetField} >= @{name}");
                            baseQueryParams.QueryParams.Add($"@{name}", value, dbType, direction: ParameterDirection.Input);
                        }
                        else 
                        {
                            baseQueryParams.QueryString.Add($"{paramAttribute.TargetField} <= @{name}");
                            baseQueryParams.QueryParams.Add($"@{name}", value, dbType, direction: ParameterDirection.Input);
                        }
                        break;
                }
            }
            return baseQueryParams;
        }
    }
}
