# PassManager API

PassManager API, kullanıcıların şifrelerini güvenli bir şekilde saklamalarını ve yönetmelerini sağlayan bir RESTful web servisidir. Bu proje, kullanıcı kimlik doğrulaması, şifrelerin gruplandırılması ve temel CRUD (Create, Read, Update, Delete) işlemlerini destekler.

## Teknoloji Yığını

* **Backend:** ASP.NET Core Web API
* **Veritabanı:** Microsoft SQL Server
* **ORM (Object-Relational Mapping):** Entity Framework Core
* **Kimlik Doğrulama:** JWT (JSON Web Tokens)
* **Şifreleme:** BCrypt - Master Key

## Özellikler

* **Kullanıcı Yönetimi:** Kayıt olma, giriş yapma, şifre değiştirme ve hesap silme.
* **Güvenli Şifre Saklama:** BCrypt kullanılarak şifrelerin hash'lenerek saklanması.
* **Grup Yönetimi:** Şifreleri kategorize etmek için gruplar oluşturma, düzenleme ve silme.
* **Şifre Yönetimi:** Kayıtlı şifrelerin eklenmesi, düzenlenmesi, silinmesi ve görüntülenmesi.
* **Loglama:** Başarılı ve başarısız kullanıcı aksiyonlarının loglanması.

## Master Key Mimarisi

**PassManager API**, kullanıcıların şifrelerini güvenli bir şekilde saklamak için bir **Master Key** (Anahtar Şifre) mimarisi kullanır. Bu yaklaşımda, her kullanıcının veritabanında saklanan tüm şifreleri, kullanıcının kendi belirlediği bir Anahtar Şifre ile şifrelenir.

* **Ekstra Güvenlik Katmanı:** Kullanıcı şifreleri, uygulamanın diğer güvenlik önlemlerine ek olarak, kullanıcının kendi şifresiyle şifrelenir. Bu, veritabanının tehlikeye girmesi durumunda bile hassas verilerin korunmasını sağlar.
* **Zero-Knowledge Prensibi:** Geliştiriciler dahil hiç kimse, kullanıcıların Anahtar Şifresini bilmeden veya erişmeden şifreleri çözemez.

---

## Kurulum

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları takip edin.

### Ön Gereksinimler

* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (veya SQL Server Express / LocalDB)
* [Visual Studio Code](https://code.visualstudio.com/) veya [Visual Studio](https://visualstudio.microsoft.com/)

### Adımlar

1.  **Veritabanı Yapılandırması:**
    `appsettings.json` dosyasını açın ve `ConnectionStrings` bölümünü kendi SQL Server bağlantı dizinize göre güncelleyin.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PassManagerDB;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

2.  **Veritabanı Migrasyonlarını Uygulayın:**
    Terminalde proje dizinindeyken aşağıdaki komutları çalıştırarak veritabanını oluşturun ve güncelleyin.
    ```bash
    dotnet ef database update
    ```
    Eğer migrasyonlar bulunmuyorsa, öncelikle `dotnet ef migrations add InitialCreate` komutuyla yeni bir migrasyon oluşturmanız gerekebilir.

3.  **Projeyi Çalıştırın:**
    ```bash
    dotnet run
    ```
    API, varsayılan olarak `https://localhost:5001` adresinde çalışacaktır. Swagger UI'a `https://localhost:5001/swagger` adresinden erişebilirsiniz.


## Mimari

Projenin katmanlı mimarisi, farklı sorumlulukları ayıran mantıksal bir yapıdır.

### Katmanlar

* **Controllers Katmanı:** Uygulamanın dış dünyaya açılan kapısıdır. Sadece HTTP isteklerini alır ve HTTP yanıtlarını döndürür. İçinde iş mantığı barındırmaz, tüm görevleri `Service` katmanına devreder.
* **Services Katmanı:** Uygulamanın ana iş mantığının kalbidir. Kontrolcülerden gelen taleplere karşılık gelen asıl işlemleri bu katmanda uygular. Birden fazla `Repository` sınıfını koordine ederek karmaşık işlemleri gerçekleştirir ve güvenlik (şifreleme, loglama) gibi konularda sorumluluk alır.
* **Repositories Katmanı:** Veri erişim mantığını soyutlar ve kapsüller. `Service` katmanı, doğrudan veritabanı sorguları yazmak yerine, bu katmanın sunduğu arayüzler (`IUserRepository`, `IGroupRepository`) üzerinden CRUD işlemlerini gerçekleştirir. Bu, veritabanı teknolojisi değiştiğinde sadece bu katmanda değişiklik yapılmasını sağlar.

### Veri Yapıları

* **Models (Modeller):** Uygulamanın veritabanındaki tabloları temsil eden sınıflardır (Entity'ler). `User`, `Group`, `Password` gibi sınıflar, veritabanı şemasının C# dilindeki karşılığıdır.
* **DTOs (Data Transfer Objects):** Veriyi katmanlar arasında ve API ile istemci arasında taşımak için kullanılan hafif veri taşıyıcı sınıflardır. Dışarıya hassas verilerin sızmasını önlemek, iç modelleri gizlemek ve farklı kullanım senaryoları için veriyi şekillendirmek gibi önemli güvenlik ve tasarım hedefleri için kullanılırlar.
* **Veri Bağlamı (ApplicationDbContext):** Entity Framework Core'un temel sınıfı olan `DbContext`'ten türetilmiştir. Veritabanındaki tabloları temsil eden `DbSet` özelliklerini içerir ve veri tabanıyla olan bağlantıyı yönetir. Repository katmanı, tüm veri erişimi için bu sınıfı kullanır.


### Temel Endpoint'ler

* `POST /api/auth/register` - Yeni kullanıcı kaydı
* `POST /api/auth/login` - Kullanıcı girişi
* `GET /api/auth/me` - Mevcut kullanıcının profil bilgilerini getir (Yetkilendirme gerektirir)
* `PUT /api/auth/change-password` - Kullanıcının şifresini değiştir (Yetkilendirme gerektirir)
* `PUT /api/auth/update-email` - Kullanıcının e-posta adresini güncelle (Yetkilendirme gerektirir)
* `DELETE /api/auth/delete-account` - Kullanıcının hesabını sil (Yetkilendirme gerektirir)
* `GET /api/groups` - Kullanıcının tüm gruplarını getir (Yetkilendirme gerektirir)
* `GET /api/groups/{id}` - Belirli bir grubu getir (Yetkilendirme gerektirir)
* `POST /api/groups` - Yeni grup oluştur (Yetkilendirme gerektirir)
* `PUT /api/groups/{id}` - Mevcut bir grubu güncelle (Yetkilendirme gerektirir)
* `DELETE /api/groups/{id}` - Mevcut bir grubu sil (Yetkilendirme gerektirir)
* `GET /api/passwords/group/{groupId}` - Belirli bir gruba ait tüm şifreleri getir (Yetkilendirme gerektirir)
* `GET /api/passwords/password/{id}` - Belirli bir şifreyi getir (Yetkilendirme gerektirir)
* `POST /api/passwords` - Yeni şifre ekle (Yetkilendirme gerektirir)
* `PUT /api/passwords/{id}` - Mevcut bir şifreyi güncelle (Yetkilendirme gerektirir)
* `DELETE /api/passwords/{id}` - Mevcut bir şifreyi sil (Yetkilendirme gerektirir)

### Youtube Linki
 * https://youtu.be/nw6WXlbYEQ0
