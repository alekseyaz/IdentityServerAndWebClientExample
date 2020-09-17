# IdentityServerAndWebClientExample

Пример связки web app с  и sts server использующий пакеты AspNetCore.ApiAuthorization.IdentityServer (внутри содержит IdentityServer4), 
AspNetCore.Identity.EntityFrameworkCore (для хранения базы юзеров и их ролей) и AspNetCore.Identity.UI (на основе Razor Pages)  
Так же прикручен serilog с симпотными скинами.

Web App https://localhost:5002 (MVVM на RazorPages) содержит ссылку на страницу Secret которая под [Authorize], при попытки зайти перебросит на Sts Server  https://localhost:5001

