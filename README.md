# BridgeRace
# Bridge Race - Unity Mobile Game Project

Bu proje Unity ile geliştirdiğim, Bridge Race oyunundan referans alarak yaptığım 3D mobil yarış oyunu.

Projeye başlarken amacım sadece oynanabilir bir oyun yapmak değildi aslında. Unity tarafında öğrendiğim OOP, Interface, Event System, Object Pooling, NavMesh, DOTween gibi sistemleri gerçek bir proje içerisinde kullanmak istiyordum.

Proje yaklaşık 4 haftalık bir geliştirme sürecinde ilerledi.

İlk başta oldukça basit görünüyordu. Karakter tuğla toplayacak, köprü yapacak ve finish'e ulaşacak gibi. Ama proje ilerledikçe AI davranışları, köprü sistemi, fizik problemleri, mobil UI, object pooling, yarış sıralaması derken düşündüğümden çok daha kapsamlı bir hale geldi.

Sonunda Android ve WebGL buildleri alınmış ve test edilmiş oynanabilir bir proje ortaya çıktı.

---

## 🎮 Oyun Hakkında

Oyunda toplam 4 yarışmacı bulunuyor.

- YOU - Oyuncu
- LIAM - Kırmızı AI
- EMMA - Yeşil AI
- NOAH - Sarı AI

Her yarışmacı kendi rengindeki tuğlaları topluyor.

Toplanan tuğlalar karakterin sırtında üst üste stackleniyor. Karakter köprüye geldiğinde sırtındaki tuğlaları kullanarak köprünün basamaklarını kendi rengine çeviriyor.

Amaç diğer yarışmacılardan önce köprüleri tamamlayıp finish noktasına ulaşmak.

---

# 🕹️ Player Movement

Oyuncu mobil kullanım düşünülerek joystick ile kontrol ediliyor.

Karakter X-Z düzleminde serbest şekilde hareket edebiliyor.

Hareket sistemi Rigidbody tabanlı çalışıyor.

Projeyi geliştirirken ilk başlarda klavye ile test yaptım. Daha sonra mobil tarafa geçince joystick sistemini ekledim.

Joystick tarafında ilk başta hareket çalışırken mobil joystick'in tepki vermediği durumlar oldu. Input ayarlarını ve Unity'nin yeni Input System ayarlarını kontrol ederek bu kısmı düzelttim.

---

# 🧱 Brick Sistemi

Oyunun en temel sistemlerinden biri tuğla sistemi.

Her karakter sadece kendi rengindeki tuğlaları toplayabiliyor.

Örneğin:

- Mavi karakter → Mavi Brick
- Kırmızı karakter → Kırmızı Brick
- Yeşil karakter → Yeşil Brick
- Sarı karakter → Sarı Brick

Tuğlalar toplandığında karakterin sırtında DOTween kullanılarak stackleniyor.

İlk yaptığım sistemde brickler spawn olurken bazıları birbirinin içine giriyordu.

Özellikle bir alanda çok fazla brick olduğunda görüntü oldukça kötü oluyordu.

Bunu daha sonra grid mantığına çevirerek çözdüm.

Bricklerin spawn noktalarını düzenli hale getirip birbirlerinin içine girmelerini engelledim.

---

# ♻️ Object Pooling

Projede brickler sürekli kullanıldığı için Instantiate ve Destroy kullanmak istemedim.

Bunun yerine Object Pooling sistemi oluşturdum.

`BrickPoolManager` üzerinden brickler havuzdan alınıyor ve kullanılmadıkları zaman tekrar havuza gönderiliyor.

Bu sistem özellikle mobil performans açısından önemliydi.

Brick toplandığında tamamen yok edilmiyor.

Pool'a geri dönüyor.

Daha sonra Coroutine sistemi ile tekrar oyun alanında spawn ediliyor.

Bu sistem sayesinde runtime sırasında sürekli GameObject oluşturup silmek yerine var olan objeleri tekrar kullanıyorum.

---

# 🧩 Interface Mimarisi

Projede gameplay detection tarafında mümkün olduğunca Interface yapısını kullandım.

Kullandığım temel interfaceler:

### ICollectable

Toplanabilir objeler için kullanılıyor.

Brick sistemi bu interface üzerinden çalışıyor.

### IBuildable

Köprü basamaklarının inşa edilebilir olup olmadığını kontrol ediyor.

Karakterin altına atılan Raycast sonucunda `IBuildable` componenti aranıyor.

### IKnockbackable

Karakterlerin birbirleriyle çarpıştığında knockback alabilmesini sağlıyor.

Bu yapı sayesinde gameplay sistemlerinde Tag kullanımına ihtiyaç kalmadı.

Objelerin ne olduğunu tag kontrolü ile değil, sahip oldukları davranış üzerinden anlamaya çalıştım.

---

# 🌉 Bridge System

Köprü sistemi projenin en fazla uğraştıran bölümlerinden biri oldu.

Karakter köprü üzerinde ilerlerken ayağının altına doğru Raycast atılıyor.

Raycast sonucunda bir `IBuildable` bulunursa karakterin sırtındaki brick sayısı kontrol ediliyor.

Brick varsa:

- Bir brick harcanıyor.
- Köprü basamağı karakterin rengine dönüşüyor.
- Karakter ilerlemeye devam ediyor.

Brick yoksa karakter ileri gidemiyor.

Bu sistem ilk başta tek Raycast ile çalışıyordu.

Ama final köprüsünde PhysicsRamp, Trigger ve başka Collider objeleri Raycast'in önüne girmeye başladı.

Karakter doğru BridgeStep'i algılamıyordu.

Bunun üzerine sistemi `Physics.RaycastAll` kullanacak şekilde değiştirdim.

Raycast'in vurduğu bütün objeleri kontrol ediyorum.

Daha sonra sadece:

`TryGetComponent<IBuildable>()`

bulunan objeyi seçiyorum.

Bu değişiklik final köprüsündeki birçok problemi çözdü.

---

# 🤖 AI Sistemi

Projede 3 farklı AI zorluk seviyesi bulunuyor.

- Easy
- Normal
- Hard

AI sistemi NavMesh kullanıyor.

Başlangıçta bütün AI davranışı tek bir `AIController` içerisinde bulunuyordu.

Proje büyüdükçe AIController çok fazla sorumluluk almaya başladı.

Bunun üzerine AI sistemini parçalara ayırdım.

Kullandığım bazı sistemler:

- `AIController`
- `AINavigation`
- `AIBrickTargeting`
- `AIBridgeTraversal`
- `AIOpponentBehaviour`
- `IAIDifficultyStrategy`

Bu sayede AI kodu daha yönetilebilir hale geldi.

---

# 🧠 Strategy Pattern

AI zorluk seviyelerinde Strategy Pattern kullandım.

Her difficulty kendi davranışını belirliyor.

### Easy

Easy AI daha rastgele oynuyor.

Brick seçiminde random davranıyor.

Toplaması gereken brick miktarı da sabit değil.

Yaklaşık 5 - 12 arasında değişebiliyor.

Ayrıca diğer karakterlerle çarpışmaktan kaçınmaya çalışıyor.

### Normal

Normal AI kendisine en yakın bricki seçiyor.

Genellikle 8 brick topladıktan sonra köprüye yöneliyor.

Rakiplere karşı özel olarak agresif veya pasif davranmıyor.

### Hard

Hard AI daha stratejik oynuyor.

Brick kümelerini değerlendirmeye çalışıyor.

Daha yoğun brick bulunan alanlara yöneliyor.

Aynı zamanda rakibinden daha fazla brick taşıyorsa oyuncunun yolunu kesmeye çalışabiliyor.

---

# 🗺️ NavMesh

AI hareketi için Unity AI Navigation ve NavMeshAgent kullandım.

Projede NavMesh tarafında da birkaç problem yaşadım.

Özellikle karakter köprüye geçerken NavMesh dışına çıkabiliyor ve Agent hedef bulamıyordu.

Bazı durumlarda AI köprü başında takılı kalıyordu.

Bunun için `NavMesh.SamplePosition` kullanarak AI'nın ulaşabileceği en yakın NavMesh noktasını bulmaya başladım.

AI respawn olduğunda da tekrar NavMesh üzerine oturmasını sağlayan kontroller ekledim.

---

# 🔄 AI Bridge Return Sistemi

AI ile ilgili en fazla uğraştığım sistemlerden biri buydu.

AI köprü yaparken brickleri biterse boş basamağın üzerinde takılı kalıyordu.

Başlangıçta AI ilerlemeye çalışmaya devam ediyordu.

Daha sonra davranışı değiştirdim.

Eğer:

- AI'nın brick sayısı 0 ise
- Önündeki köprü basamağı boş ise

AI ileri gitmek yerine geri dönüyor.

Tekrar brick alanına gidiyor.

Brick topluyor.

Daha sonra kaldığı köprüye tekrar dönüp ilerlemeye devam ediyor.

Bu sistem özellikle AI'nın yarışı kendi başına tamamlayabilmesi açısından önemli oldu.

---

# 💥 Knockback Sistemi

Karakterler birbirleriyle çarpışabiliyor.

Çarpışma sırasında iki karakterin taşıdığı brick sayısı karşılaştırılıyor.

Daha az brick taşıyan karakter knockback alıyor.

Karakter geriye doğru savruluyor ve taşıdığı bricklerin bir kısmını / tamamını düşürebiliyor.

Knockback sistemi `IKnockbackable` interface üzerinden çalışıyor.

AI ve Player arasında çift yönlü çalışacak şekilde yapıldı.

Yani sadece Player AI'yı deviremiyor.

AI da şartlar oluştuğunda Player'ı devirebiliyor.

---

# 🏆 Finish ve Podium Sistemi

Finish tarafında yarışmacıların sıralaması takip ediliyor.

Bir yarışmacı finish'e ulaştığında EventManager üzerinden yarış sistemine haber veriliyor.

İlk üç yarışmacı podium üzerine gönderiliyor.

Podium sıralaması:

1. Place
2. Place
3. Place

şeklinde.

Karakterler podium üzerine giderken DOTween animasyonu kullanılıyor.

Finish sonrası karakterin normal hareketi durduruluyor ve fizik davranışları kapatılıyor.

Kazanan karakter ayrıca Winner animasyonuna geçiyor.

---

# 📊 Live Leaderboard

Gameplay sırasında sağ üst bölümde canlı bir sıralama sistemi bulunuyor.

İlk versiyonda leaderboard üzerinde hem karakter isimlerini hem de taşıdıkları brick miktarını gösteriyordum.

Örneğin:

`1. YOU - 12`

gibi.

Editor içerisinde güzel görünüyordu ama Android build aldıktan sonra mobil ekranda yazılar sağa sola kaymaya başladı.

UI gereksiz kalabalık görünüyordu.

Bu yüzden sistemi sadeleştirdim.

Artık sadece yarışmacı isimleri gösteriliyor.

Örneğin:

YOU  
LIAM  
EMMA  
NOAH

Ancak sıralama sistemi arka planda hala canlı olarak çalışıyor.

Bir karakter öne geçtiğinde isimlerin sırası değişiyor.

Leaderboard UI doğrudan gameplay sistemine bağlı değil.

EventManager üzerinden gelen eventleri dinliyor.

---

# 📡 Event Driven Architecture

Projede UI ve gameplay sistemlerinin birbirine mümkün olduğunca doğrudan bağlı olmamasına dikkat ettim.

Bu yüzden merkezi bir `EventManager` kullandım.

Bazı sistemler:

- Brick Collected
- Character Knockback
- Character Finish
- Character Placement
- Combo
- Leaderboard
- Victory Gold

EventManager üzerinden haberleşiyor.

UI scriptleri genellikle `OnEnable` içerisinde eventlere subscribe oluyor.

`OnDisable` içerisinde unsubscribe oluyor.

Bu sayede özellikle sahne yeniden yüklendiğinde eventlerin birden fazla kez çalışması gibi sorunların önüne geçmeye çalıştım.

---

# 🔥 Brick Combo Sistemi

Projeye kendim ekstra bir mekanik olarak Combo sistemi ekledim.

Oyuncu arka arkaya 5 normal brick topladığında:

`COMBO! +2`

bonusunu kazanıyor.

Yani oyuncunun stackine ekstra 2 brick ekleniyor.

Burada özellikle bonus bricklerin tekrar combo sayacına dahil olmaması gerekiyordu.

Yoksa:

5 brick → +2 bonus → bonus brickler de combo sayılır → tekrar combo

gibi sonsuz bir zincir oluşabilirdi.

Bu yüzden normal brick collection eventleri ile bonus brick ekleme işlemini birbirinden ayırdım.

Knockback alındığında combo sıfırlanıyor.

Combo UI animasyonlarında DOTween kullandım.

---

# 💰 Gold Sistemi

Oyun sonunda oyuncuya Gold ödülü veriliyor.

Temel ödül:

`100 Gold`

Oyuncunun finish sonunda sırtında brick kaldıysa her brick ekstra 2 Gold kazandırıyor.

Formül:

`100 + Remaining Brick Count * 2`

Örneğin oyuncunun 10 bricki kalmışsa:

`100 + 10 * 2 = 120 Gold`

Gold miktarı PlayerPrefs ile kaydediliyor.

---

# 🪙 Victory Gold Animation

Victory ekranındaki Gold sistemi için ayrıca UI animasyonu yaptım.

Ödül ilk olarak ekranın orta bölümünde gösteriliyor.

Daha sonra coinler ekranda beliriyor.

Coinler DOTween kullanarak tek tek sağ üstte bulunan Gold Wallet bölümüne uçuyor.

Her coin wallet'a ulaştığında toplam Gold değeri animasyonlu şekilde artıyor.

Mobil build aldıktan sonra bu animasyonları tekrar test ettim çünkü Editor ve gerçek cihaz UI davranışları aynı olmayabiliyor.

---

# 💾 Save System

Projede bazı değerler PlayerPrefs ile kaydediliyor.

Bunlardan bazıları:

- Seçilen AI Difficulty
- Player Gold

Oyuncu oyunu kapatıp tekrar açtığında Gold miktarı korunuyor.

Difficulty seçimi de kayıt altına alınıyor.

Save işlemleri için ayrı bir `SaveManager` oluşturdum.

---

# 🎚️ Difficulty Selection

Oyuna başlamadan önce oyuncu:

- EASY
- NORMAL
- HARD

seçeneklerinden birini seçebiliyor.

Seçilen difficulty sadece AI hızını değil AI'nın brick arama ve rakip davranışlarını da etkiliyor.

Difficulty seçildikten sonra oyun sahnesi açılıyor.

---

# 🖥️ Main Menu

Oyunun basit bir Main Menu sistemi bulunuyor.

Oyun akışı:

Main Menu  
↓  
Difficulty Select  
↓  
Gameplay

şeklinde ilerliyor.

UI butonlarında DOTween animasyonları kullandım.

---

# 🎥 Camera

Gameplay kamerası karakteri takip ediyor.

Harita büyüdükçe kamera tarafında da birkaç kez değişiklik yapmak zorunda kaldım.

Özellikle final bridge ve podium sisteminden sonra kamera pozisyonunu tekrar ayarladım.

Mobil portrait görünümünde hem Player'ın hem de önündeki köprünün rahat görülebileceği bir kamera açısı oluşturmaya çalıştım.

---

# 🎵 Audio

Projede temel ses efektleri bulunuyor.

Örneğin:

- Brick Collect
- Bridge Build
- Victory

gibi olaylarda ses oynatılıyor.

Ses sistemlerini de gameplay eventlerinden tetikleyerek mümkün olduğunca sistemleri birbirinden bağımsız tutmaya çalıştım.

---

# 📱 Mobile UI

Proje Android Portrait düşünülerek geliştirildi.

Test çözünürlüğü:

`1080 x 1920`

Mobil UI tarafında özellikle Anchor ve Canvas Scaling ayarları önemli oldu.

Editor içerisinde düzgün görünen bazı UI elemanları APK build sonrasında farklı davranabiliyordu.

Leaderboard bunun en net örneğiydi.

Bu yüzden Android cihaz üzerinde ayrıca test yapıp UI tarafını sadeleştirdim.

---

# 🛠️ Geliştirme Sürecinde Karşılaştığım Bazı Problemler

Bu proje aslında bana en fazla hata çözme konusunda şey öğretti diyebilirim.

Çünkü bazı sistemler ilk yazdığım anda çalışmadı.

Bazıları çalıştı ama proje büyüyünce bozuldu.

Bazıları Editor'de çalıştı telefonda farklı göründü.

Karşılaştığım bazı problemler:

### Bricklerin üst üste spawn olması

İlk BrickSpawner sisteminde brickler birbirlerinin içine giriyordu.

Grid tabanlı spawn düzeni oluşturarak çözdüm.

---

### AI'nın köprüde brick bitince takılması

AI'nın brickleri bitmesine rağmen ileri gitmeye çalışması büyük problemlerden biriydi.

Bridge state sistemine geri dönüş state'leri ekleyerek çözdüm.

AI artık bricki kalmadığında toplama alanına geri dönüyor.

---

### AI'nın NavMesh dışına çıkması

Özellikle köprü geçişlerinde Agent bazı noktalarda NavMesh üzerinde kalmıyordu.

`NavMesh.SamplePosition` ve Warp mantığı ile Agent'ı tekrar geçerli NavMesh pozisyonuna taşıdım.

---

### Final Bridge Raycast problemi

Final Bridge üzerinde farklı colliderlar bulunduğu için Raycast her zaman doğru BridgeStep'e çarpmıyordu.

Tek Raycast yerine `RaycastAll` kullanarak bütün sonuçları kontrol ettim.

Sadece `IBuildable` olan colliderları değerlendirdim.

---

### AIController'ın çok büyümesi

İlk zamanlarda bütün AI davranışını tek bir script içerisinde yazmıştım.

Kod büyüdükçe kontrol etmek oldukça zorlaştı.

AI sistemini:

Navigation  
Brick Targeting  
Bridge Traversal  
Opponent Behaviour  
Difficulty Strategy

gibi bölümlere ayırdım.

Bu proje içerisinde OOP'nin neden önemli olduğunu en net anladığım yerlerden biri burası oldu.

---

### Duplicate Serialized Field Hatası

Projenin sonlarına doğru Unity şu hatayı vermeye başladı:

`The same field name is serialized multiple times`

Sorunun `CharacterBase` ve `AIController` içerisinde aynı `bridgeBuilder` değişkeninin bulunmasından kaynaklandığını fark ettim.

`bridgeBuilder` referansını CharacterBase içerisinde `protected` hale getirerek AIController'ın aynı referansı inheritance üzerinden kullanmasını sağladım.

Böylece aynı component için iki farklı field tutmaya gerek kalmadı.

---

### Kullanılmayan değişken warningi

Final kontrolde Console'da:

`CS0414`

warningi vardı.

Eski AI sisteminden kalan `bridgeReachedDistance` değişkeninin artık hiçbir yerde kullanılmadığını gördüm.

Değişkeni kaldırıp Console'u temizledim.

---

### Leaderboard mobil UI problemi

Leaderboard içerisinde ilk başta brick sayıları da bulunuyordu.

Editor ekranında düzgün görünüyordu.

Android build aldıktan sonra yazıların hizası bozuldu.

Bir süre layout ayarlarıyla uğraştım ama daha sonra mobil için daha doğru kararın UI'yı sadeleştirmek olduğuna karar verdim.

Brick sayılarını kaldırdım.

Live ranking sistemi çalışmaya devam etti ama ekranda sadece isimler kaldı.

Bence daha temiz oldu.

---

### Karakter başlangıç pozisyonları

Bazı AI karakterlerinin başlangıç pozisyonları birbirine çok yakın olduğu için yarış başladığında birbirlerine çarpıyorlardı.

Başlangıç noktalarını tekrar düzenlemek zorunda kaldım.

---

### Podium pozisyonları

Finish sistemini ilk yaptığımda karakterlerin podium üzerindeki konumları istediğim gibi değildi.

Birinci, ikinci ve üçüncü sıra noktalarını tekrar ayarladım.

DOTween hareketi ile karakterleri ilgili podium noktasına taşıdım.

---

### AI çarpışmalarının fazla olması

İlk AI sisteminde botlar birbirlerine çok fazla yaklaşıyordu.

Bu hem takılmalarına hem de sürekli knockback oluşmasına neden oluyordu.

Difficulty davranışlarını geliştirip Easy modda avoidance, Hard modda agresif davranış oluşturdum.

---

# 🧪 Testing

Projede sadece Unity Editor içerisinde test yapmadım.

Final aşamasında:

### Android

APK build alındı.

Gerçek Android cihaz üzerinde test edildi.

Kontrol edilen sistemler:

- Main Menu
- Difficulty
- Joystick
- Brick Collect
- Stacking
- Bridge Build
- AI
- Knockback
- Leaderboard
- Combo
- Finish
- Podium
- Gold
- Save System
- Victory
- Continue

### WebGL

WebGL build alındı.

Browser üzerinde test edildi.

---

# 💻 Kullanılan Teknolojiler

- Unity 6
- C#
- Unity AI Navigation
- NavMesh
- Rigidbody Physics
- DOTween
- TextMeshPro
- PlayerPrefs
- Object Pooling
- Interfaces
- Observer Pattern
- Strategy Pattern
- State Machine
- Coroutine
- Raycast / RaycastAll
- Git
- GitHub

---

# 🧱 Projede Kullandığım Bazı Mimari Yapılar

### OOP / Inheritance

`CharacterBase`

sınıfından Player ve AI karakterleri türetiliyor.

Ortak hareket ve karakter davranışlarının tekrar tekrar yazılmasını engellemeye çalıştım.

### Interface

`ICollectable`

`IBuildable`

`IKnockbackable`

### Observer Pattern

`EventManager`

Gameplay ve UI sistemlerinin haberleşmesi için.

### Strategy Pattern

AI difficulty sisteminde kullanıldı.

### Object Pool Pattern

Brick oluşturma ve yeniden kullanma sisteminde kullanıldı.

### State Machine

AI'nın:

Brick Collect  
Bridge  
Return  
Middle Area  
Final Bridge  
Finish

gibi durumları arasında geçiş yapması için kullanıldı.

---

# 📦 Build

Proje final aşamasında iki platform için build edildi.

- Android ✅
- WebGL ✅

İki build de test edildi.

---

# 📚 Bu Projede Neler Öğrendim?

Bu projede en çok fark ettiğim şey bir özelliğin çalışmasının tek başına yeterli olmadığı oldu.

Başlangıçta kod çalışıyorsa tamam diye düşünüyordum.

Ama proje büyüdükçe kodun nasıl organize edildiği çok daha önemli hale geliyor.

Özellikle AIController büyüdüğünde bunu bayağı hissettim.

Bir şeyi tek scriptte yapmak ilk başta daha hızlı geliyor ama proje ilerlediğinde değiştirmek zorlaşıyor.

Interface, Strategy, Event System ve Object Pooling gibi yapıların neden kullanıldığını bu projeyi yaparken daha iyi anladım.

Bir diğer önemli konu mobil test oldu.

Unity Game ekranında düzgün görünen şeyin gerçek Android cihazda aynı görünmeyebileceğini gördüm.

UI tarafında gerçek cihaz testi gerçekten gerekliymiş.

Ve tabi hata çözme kısmı.

Bazen küçük görünen bir collider ayarı veya bir serialized field bütün sistemi etkileyebiliyor.

Projede en fazla zaman alan şey kod yazmaktan çok bazı hataların neden oluştuğunu bulmak oldu diyebilirim.

Ama zaten en çok öğrendiğim kısım da orası oldu.

---

# 🚀 Proje Durumu

Project Status:

**Completed ✅**

Android Build: **Tested ✅**

WebGL Build: **Tested ✅**

---

Bu proje benim Unity tarafında yaptığım en kapsamlı projelerden biri oldu.

Sadece gameplay yapmaya değil, kod mimarisini olabildiğince düzenli tutmaya ve mobil ortamda gerçekten çalışan bir oyun ortaya çıkarmaya çalıştım.

Hala geliştirilebilecek noktaları tabi ki var.

Ama başladığım noktayla final hali arasında ciddi fark olduğunu düşünüyorum.

Bu proje sayesinde özellikle Unity, C#, OOP, AI ve mobil oyun geliştirme tarafında çok fazla pratik yapmış oldum.
