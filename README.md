# Restaurant Management System (.NET Framework 4.7.2, WinForms)

Ky projekt është një sistem desktop për menaxhimin e restorantit, i ndërtuar me C# + WinForms, me arkitekturë të shtresuar dhe SQL Server.

## Arkitektura

Solution përmban 5 projekte:

- `RestaurantManagementSystem.UI` – WinForms (Login, Dashboard, Tavolina, Menu, Porosi, Staf, Raporte, Faturë)
- `RestaurantManagementSystem.BLL` – logjika e biznesit dhe validimet
- `RestaurantManagementSystem.DAL` – akses në databazë me ADO.NET
- `RestaurantManagementSystem.Models` – entitetet dhe enum-et
- `RestaurantManagementSystem.Services` – shërbime ndihmëse (NFC/RFID mock + printim)

## Funksionalitetet Kryesore

### Admin
- Menaxhon tavolinat (shto/fshi/ndrysho status)
- Menaxhon artikujt e menusë
- Shikon raportet ditore dhe mujore
- Menaxhon stafin dhe oraret
- Dashboard me statistika të shpejta

### Waiter
- Login me username/password
- Login me kartë NFC/RFID (mock)
- Hap porosi, shton/heq artikuj
- Transferon porosinë nga një tavolinë tek tjetra
- Kryen pagesë, llogarit kusurin automatikisht
- Preview dhe printim fature

## Databaza

Skriptet SQL ndodhen te:

- `RestaurantManagementSystem.Database/init.sql`

Tabela të përfshira:
- Roles
- Users
- Waiters
- StaffSchedules
- RestaurantTables
- MenuCategories
- MenuItems
- Orders
- OrderItems
- Payments

## Si të ekzekutohet

1. Hap `RestaurantManagementSystem.sln` në Visual Studio 2019/2022 (Windows).
2. Sigurohu që target framework është `.NET Framework 4.7.2`.
3. Ekzekuto skriptin `RestaurantManagementSystem.Database/init.sql` në SQL Server.
4. Përditëso connection string në `DatabaseContext.cs` nëse serveri yt nuk është `Server=.;Database=RestaurantManagementDb;Trusted_Connection=True;`.
5. Vendos `RestaurantManagementSystem.UI` si Startup Project dhe run.

## Kredenciale seed

- Admin:
  - username: `admin`
  - password: `admin123`
- Waiter:
  - username: `waiter1`
  - password: `waiter123`
  - card UID mock: `MOCK-WAITER-CARD-001`

## Shënime për performancë

- UI me WinForms, forma të thjeshta dhe ngarkim i shpejtë.
- ADO.NET direkt pa ORM të rëndë për overhead minimal.
- Queries të fokusuara dhe struktura e qartë për mirëmbajtje dhe zgjerim në të ardhmen.
