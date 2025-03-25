# 執行步驟

1. 描繪使用者會需要的介面，如使用者會需要哪些 input、output 資料。
2. 設計資料庫，將使用者會需要的 input 設計可存入資料庫的欄位。
3. Store Procedure 開發，使用 User-Defined Table Type 進行集合物件傳入，訂單建立時使用 Transaction 確保訂單主表與明細表同時寫入成功。
4. API 開發，建立 Dao、Service、Controller、DI。
5. API 測試，使用 Postman 進行測試。
6. Web 開發，建立 Route、Menu、Page、Component。
7. Web 測試，手動測試。
8. 權限設計與開發，使用 AuthorizationFilter 與 JWT Token 進行權限驗證。
9. API 傳入參數格式驗證，使用 ActionFilter、ExceptionFilter、Attribute 進行驗證與訊息回覆。
10. 手動整合測試