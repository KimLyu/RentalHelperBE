## 專題開發-租售電商平台_租之助

後端 API 服務器部分

使用ASP.Net Core 進行開發，
連接後端MySQL資料庫，

## 技術使用重點

- Entity Framework Core
  - Code First模式，透過migrations 進行資料庫建表。
  - 用作資料庫對應Model，使用標籤設定主Key、表單關聯、字數限制等。
  - 利用導航、反向導航，擷取資源資訊，透過AutoMapper進行資訊傳遞及轉換。
- MVC控制器  - Controller模式
  - 將不同的資源服務，分開設定到不同的控制器，設定各種資源的響應行為。
  - 使用FromHeader取JWT使用;
  - FromBody使用JsonElement取資訊，或使用Model直接取用對應物件。
  - 進行產品資料刪除時，訂單不該同時進行關聯刪除，故需要在產品刪除時，
    解除關聯關係(將反向關聯設定為Null)，並在訂單取出時，於profile填入預設 "查無產品"圖片。
- 數據倉庫模式 - Repository
  - 利用 注入方式，將資料庫對應資源，各自對應Repository資源。
  - 將各數據模型對應資料庫行為，在Ropository建立。
- 資料對應輸出 - AutoMapper
  - 使用DTO模型，對應Controller所需要回覆的資料類型。
  - 透過AutoMapper的Profile所設定的轉換，擷取資料庫資訊，並轉換為DTO格式輸出。
- SMTP 使用
  - 尚未架設內部MailServer，僅在Google註冊一個測試用帳號，並設定低安全性使用
  - 使用SMTP設定，讓GMail進行驗證需求的寄信功能。
  - 目前僅使用英文大小寫及數字，使用隨機產生8碼，並儲存於資料庫中。
- 圖檔上傳 - Base64
  - 請前端 將圖片轉換為Base64格式上傳。
  - 收到資訊後，先確認是否為Base64格式，轉回並載入為BMP，再進行服務器端的儲存，
  - 正確儲存後，僅將路徑儲存到資料庫。
- 服務器端使用Apache2 進行動態API及靜態圖片庫的代理。
- BCrypt 加密 
  - 將註冊的密碼資訊，進行Hash轉換後儲存，避免資料庫被攻擊洩漏時，密碼直接流出。
  - 登入時，進行單向Hash轉換，再比對轉換後的編碼是否正確。
- JWT 使用
  - 使用JWT進行身分認證後的操作，必須正確登入後，取得Token才能進行個人帳號操作。
  - 僅使用基礎 JWT ，尚未使用Salt，尚未加入ReToken。
- dotnet 發布時需轉換為Linux使用，並上傳到GCP所設定VM的Ubuntu內。
- linux上需使用Supervisord 進行dotnet 程序的管控，避免異常時，整個服務器停止。
- 未使用排程進行圖片庫清理、訂單清理，目前僅設定一支工具用API作為圖片庫清掃(查無物件關聯則刪除該檔案)。