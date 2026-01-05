# 🤖 InsureYouAI  (AI-Powered Insurance Platform)
### **Çoklu Yapay Zekâ Entegrasyonlu Sigorta Platformu  (ASP.NET Core + MSSQL + Multi-AI + SignalR + PDF + Vision + NLP + Email Automation)**

---

## 🚀 Proje Hakkında

**InsureYouAI**, sigorta sektörüne özel olarak geliştirilmiş, yapay zekâ entegrasyonlarını merkeze alan **modern bir ASP.NET Core** projesidir.  
Klasik CRUD yaklaşımının ötesine geçerek; **AI ile içerik üretimi**, **AI tabanlı kullanıcı analitiği**, **PDF poliçe analizi**, **görselden hasar tespiti (Vision AI)**, **web destekli araştırma (Tavily)**, **gerçek zamanlı AI Chat (SignalR + streaming)** ve **AI ile otomatik e-posta yanıtı** gibi gerçek dünya senaryolarını tek bir platformda birleştirir.

🎓 Bu proje, Udemy’de yayınlanan **“ASP.NET Core ile Yapay Zeka Entegrasyonları – Sigorta Projesi”** eğitimi temel alınarak geliştirilmiştir.

---

## 📌 İçindekiler
- [Öne Çıkan Senaryolar](#-öne-çıkan-senaryolar)
- [Modüller ve AI Entegrasyonları](#-modüller-ve-ai-entegrasyonları)
- [Kullanılan Teknolojiler](#-kullanılan-teknolojiler)
- [Kurulum](#-kurulum)
- [Yapılandırma](#-yapılandırma)
- [Ekran Görüntüleri](#-ekran-görüntüleri)
- [Geliştirici](#-geliştirici)

---

## 🧠 Öne Çıkan Senaryolar

### 📰 1) AI ile Makale Üretimi (OpenAI)
- Kullanıcıdan gelen özet/anahtar kelime prompt’u ile **sigortacılık sektörüne uygun** içerik üretimi
- En az **5000 karakter** içerik
- 429 (TooManyRequests) için **retry mekanizması**

### 👤 2) AI ile Kullanıcı Profil Analizi (OpenAI)
- Bir kullanıcının yazdığı **tüm makaleleri** toplayıp tek metne çevirir
- Profesyonel rapor formatında:
  - konu çeşitliliği, hedef kitle, dil/anlatım, doğruluk, CTA, aksiyon listesi vb.

### 💬 3) AI ile Yorum Davranış Analizi (OpenAI)
- Kullanıcının yaptığı **tüm yorumları** analiz eder
- Duygu durumu, toksik içerik, iletişim tarzı, ilgi alanları vb. raporlar


### 📨 4) AI ile Müşteri Mesajı Sınıflandırma + Otomatik Yanıt + Mail Gönderimi (Gemini)
- Müşteri mesajı kaydedilir
- AI ile **kategori** ve **öncelik** tespiti (AiCategory + Priority)
- Gemini ile “müşteri temsilcisi gibi” yanıt üretimi (**tek satır JSON**: `subject`, `body`)
- SMTP ile müşteriye mail gönderimi
- DB’ye log kaydı

### 📄 5) Claude ile PDF Poliçe Analizi (PdfPig + Claude)
- PDF yükleme
- PdfPig ile metin çıkarma
- Claude Messages API ile Markdown çıktısı:
  - 10 maddede özet
  - neleri kapsar / kapsamaz
  - kritik uyarılar **kalın**

### 🚗 6) Claude Vision ile Hasar Tespit (Görsel Analizi)
- Hasar fotoğrafı yükleme
- Claude Vision API ile eksper raporu:
  - hasar türü, etkilenen alan, şiddet, onarım süreci, eksper notu

### 🧾 7) AI Destekli Sigorta Paket Önerisi (OpenAI)
- Kullanıcı profili JSON’a çevrilir
- Sadece **geçerli JSON** döndürür:
  - `onerilenPaket`, `ikinciSecenek`, `neden`

### 🧩 8) Google Gemini ile “Hakkımızda Öğeleri” Üretimi
- Kurumsal sigorta firması için güven verici **en az 10 adet** about item üretir
- 503 (ServiceUnavailable) için **retry** mekanizması

---

## 🧩 Modüller ve AI Entegrasyonları

Aşağıdaki modüller projede aktif olarak bulunmaktadır (Controller bazında):

### ✅ OpenAI
- **CreateArticleWithOpenAI**: Sigorta makalesi üretir (gpt-4.1-mini) + 429 retry  
- **UserProfileWithAI**: Kullanıcının makalelerine göre içerik tarzı analizi (gpt-4o-mini)  
- **UserCommentsProfileWithAI**: Kullanıcı yorum davranış analizi (gpt-4o-mini)  
- **ImageAIController**: Görsel üretimi (gpt-image-1) + galeri  

### ✅ Google Gemini
- **SendMessage**: Müşteri mesajına profesyonel yanıt JSON’u üretir + mail gönderir  
- **CreateAboutItemWithGoogleGemini**: Hakkımızda öğeleri üretir + 503 retry  

### ✅ Anthropic Claude
- **PolicyAnalysisWithClaudeController**: PDF poliçe analizi (PdfPig + Claude Messages API)  
- **DamageAssessmentController**: Hasar fotoğrafı analizi (Claude Vision)  

### ✅ Tavily
- Web tarama + güvenilir kaynaklardan özetleme (admin panel senaryosu)

### ✅ ElevenLabs
- Text-to-Speech senaryosu (AI çıktısını seslendirme)

---

## ⚙️ Kullanılan Teknolojiler

<p align="center">
  <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/MSSQL-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
  <img src="https://img.shields.io/badge/EF%20Core-Code%20First-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/SignalR-Real--Time-1E90FF?style=for-the-badge&logo=microsoft&logoColor=white" />
  <img src="https://img.shields.io/badge/OpenAI-gpt--4.1--mini%20%7C%20gpt--image--1-111111?style=for-the-badge&logo=openai&logoColor=white" />
  <img src="https://img.shields.io/badge/Anthropic-Claude%203.7%20Sonnet-101010?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Google%20Gemini-gemini--2.5--flash-4285F4?style=for-the-badge&logo=google&logoColor=white" />
  <img src="https://img.shields.io/badge/Tavily-Web%20Search-2E7D32?style=for-the-badge" />
  <img src="https://img.shields.io/badge/ElevenLabs-Text--to--Speech-FF6B6B?style=for-the-badge" />
  <img src="https://img.shields.io/badge/PdfPig-PDF%20Parsing-orange?style=for-the-badge" />
</p>

---


## 📸 Ekran Görüntüleri

### 🧭 Gösterge Paneli (Dashboard)

![Dashboard](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/dashboard.png)
![Dashboard1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/dashboard1.png)
![Dashboard2](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/dashboard2.png)

---

### 📰 Makale ve İçerik Yönetimi (OpenAI)

![ArticleList](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/article.png)
![CreateArticleWithOpenAI](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/CreateArticleWithOpenAI.png)

![ArticleUI](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/aticleUI.png)
![ArticleUI1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/articleUI1.png)
![ArticleDetail](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/articledetail.png)


---

### 👤 Kullanıcılar ve Yapay Zekâ Profil Analizi

![UserList](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/userlist.png)
![UserProfileWithAI](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UserProfileWithAI.png)

![Comments](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/comment.png)
![UserCommentsProfileWithAI](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UserCommentsProfileWithAI.png)

---

### 💬 Gerçek Zamanlı Yapay Zekâ Sohbeti 

![Chatbot](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/chatbot.png)

---

### 📨 Gemini – Otomatik Yanıt ve E-Posta Akışı

![SendMessage1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/SendMessage1.png)
![MessageList](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/messagelist.png)
![SendMessage](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/SendMessage.png)

---

### 📄 Claude – PDF Poliçe Analizi

![PdfAnalysis1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/pdfanaliz1.png)
![PdfAnalysis2](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/pdfanaliz2.png)

![AnalyzePolicyWithClaude](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/AnalyzePolicyWithClaude.png)
![AnalyzePolicyWithClaude1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/AnalyzePolicyWithClaude1.png)

---

### 🚗 Claude Vision – Hasar Tespit ve Değerlendirme

![DamageAssessment](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/DamageAssessment.png)
![DamageAssessment1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/DamageAssessment1.png)
![DamageAssessment2](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/DamageAssessment2.png)

---

### 🧾 Yapay Zekâ Destekli Sigorta Paket Önerisi

![CreateUserCustomizePlan](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/CreateUserCustomizePlanAI.png)
![CreateUserCustomizePlan1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/CreateUserCustomizePlanAI.png1.png)

---

### 🔍 Tavily – Web Arama ve Araştırma

![Tavily](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/tavily.png)

---

### 🔊 ElevenLabs – Metinden Sese Dönüştürme (TTS)

![ElevenLabs](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/elevenlab.png)

---

### 📈 Tahminleme ve Forecasting Modülü

![Forecasting](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/forecasting.png)

---

### 🧩 Kurumsal Sayfalar ve UI Bileşenleri

![About](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/about.png)
![AboutItem](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/aboutitem.png)
![Services](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/services.png)

![Contact](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/Contact.png)
![Pricing](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/pricingplan.png)
![Slider](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/slider.png)
![Testimonial](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/testimonial.png)
![Category](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/category.png)

---

### 🎨 OpenAI Görsel Üretimi ve Galeri

![AllImage](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/AIImage.png)

---

### 🧱 UI / Landing / Şablon Sayfalar

![UI](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UI.png)
![UI1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UI1.png)
![UI2](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UI2.png)
![UI4](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UI4.png)
![UI5](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UI5.png)
![UI6](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UI6.png)
![UI7](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UI7.png)
![UI8](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/UI8.png)

---

### 🚫 Hata Sayfası

![404](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/404.png)


---

## 👨‍💻 Geliştirici

**Cevdet Karakulak**  
🧩 Full Stack Developer  
🔗 LinkedIn: https://www.linkedin.com/in/cevdet/

---
