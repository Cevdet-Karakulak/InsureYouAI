# 🤖 InsureYouAI  
### **AI-Powered Insurance Platform (ASP.NET Core + MSSQL + Multi-AI + SignalR + PDF + Vision + NLP)**

<p align="center">
  <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/MSSQL-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
  <img src="https://img.shields.io/badge/EF%20Core-Code%20First-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/SignalR-Real--Time-1E90FF?style=for-the-badge&logo=microsoft&logoColor=white" />
  <img src="https://img.shields.io/badge/OpenAI-gpt--4.1--mini%20%7C%20gpt--image--1-111111?style=for-the-badge&logo=openai&logoColor=white" />
  <img src="https://img.shields.io/badge/Anthropic-Claude%203.7%20Sonnet-101010?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Tavily-Web%20Search-2E7D32?style=for-the-badge" />
  <img src="https://img.shields.io/badge/ElevenLabs-Text--to--Speech-FF6B6B?style=for-the-badge" />
  <img src="https://img.shields.io/badge/PdfPig-PDF%20Parsing-orange?style=for-the-badge" />
</p>

---

## 🚀 Proje Hakkında

**InsureYouAI**, sigorta sektörüne özel olarak geliştirilmiş, yapay zekâ entegrasyonlarını merkeze alan **modern bir ASP.NET Core** projesidir.  
Klasik CRUD yaklaşımının ötesine geçerek; **AI ile içerik üretimi**, **yorum moderasyonu**, **kullanıcı analizi**, **PDF poliçe analizi**, **görselden hasar tespiti (Vision AI)**, **web destekli araştırma (Tavily)** ve **gerçek zamanlı AI Chat (SignalR + streaming)** gibi gerçek dünya senaryolarını tek bir platformda toplar.

🎓 Bu proje, Udemy’de yayınlanan  
**“ASP.NET Core ile Yapay Zeka Entegrasyonları – Sigorta Projesi”**  
eğitimi kapsamında geliştirilmiştir.

---

## 🎯 Projenin Amacı

- Sigorta gibi **kurumsal bir domain** üzerinde AI entegrasyonlarını uygulamak  
- Bir projede **birden fazla AI sağlayıcısını** (OpenAI, Claude, Tavily, ElevenLabs) birlikte kullanmak  
- **Gerçek zamanlı AI deneyimi** sunmak (SignalR + streaming)  
- Portföyde güçlü duracak **AI-first backend** geliştirme pratiği kazanmak  

---

## 🧠 Öne Çıkan Özellikler

### 📰 İçerik & CMS
- Makale / kategori yönetimi
- Sigorta içeriklerini UI tarafında listeleme
- Detay sayfaları ve modern tema yapısı

### 💬 Yorum Yönetimi
- Yorum listeleme & moderasyon
- AI tabanlı analiz senaryoları

### 🤖 Gerçek Zamanlı AI Chat
- SignalR ile canlı chatbot
- Streaming yanıt mantığı

### 🎨 OpenAI Görsel Üretimi
- `gpt-image-1` ile prompt tabanlı görsel üretimi
- Üretilen görsellerin galeri halinde sunulması

### 📄 Claude ile PDF Poliçe Analizi
- PdfPig ile PDF metin çıkarma
- Claude Messages API ile:
  - 10 maddede özet
  - Kapsar / kapsamaz
  - Kritik uyarılar (**bold**)
  - Markdown formatlı çıktı

### 🚗 Claude Vision ile Hasar Tespit
- Hasar fotoğrafı yükleme
- Claude Vision API ile:
  - Hasar türü
  - Etkilenen alanlar
  - Şiddet analizi
  - Tahmini onarım süreci
  - Eksper notu

### 🔍 Tavily + OpenAI (Web Destekli AI)
- Güncel veriye dayalı arama
- Kanıta dayalı AI cevapları
- Halüsinasyon azaltma yaklaşımı

### 🔊 ElevenLabs (Text-to-Speech)
- AI çıktılarının seslendirilmesi

### 💳 AI Destekli Sigorta Paket Önerisi
- Kullanıcı profilinden JSON üretimi
- OpenAI ile:
  - Önerilen paket
  - Alternatif paket
  - Kısa analiz

---

## ⚙️ Kullanılan Teknolojiler

<p align="center">
  <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/MSSQL-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
  <img src="https://img.shields.io/badge/EF%20Core-Code%20First-5C2D91?style=for-the-badge" />
  <img src="https://img.shields.io/badge/SignalR-Real--Time-1E90FF?style=for-the-badge" />
  <img src="https://img.shields.io/badge/OpenAI-GPT%20%26%20Images-111111?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Anthropic-Claude%20Messages%20%26%20Vision-101010?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Tavily-Web%20Search%20API-2E7D32?style=for-the-badge" />
  <img src="https://img.shields.io/badge/ElevenLabs-TTS-FF6B6B?style=for-the-badge" />
  <img src="https://img.shields.io/badge/PdfPig-PDF%20Parsing-orange?style=for-the-badge" />
</p>

---

## 📁 Ekran Görüntüleri  
📌 Kaynak klasör: `InsureYouAI/wwwroot/SS`

### 📊 Dashboard
![dashboard](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/dashboard.png)
![dashboard1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/dashboard1.png)
![dashboard2](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/dashboard2.png)

### 📰 Makaleler & UI
![articleUI](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/articleUI.png)
![articledetail](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/articledetail.png)

### 🤖 SignalR Chat
![chatbot](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/chatbot.png)

### 💳 AI Paket Önerisi
![pricingplan](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/pricingplan.png)
![CreateUserCustomizePlan](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/CreateUserCustomizePlan.png)

### 🎨 OpenAI Image
![AIImage](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/AIImage.png)

### 📄 Claude PDF Analizi
![pdfanaliz1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/pdfanaliz1.png)
![AnalyzePolicyWithClaude](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/AnalyzePolicyWithClaude.png)

### 🚗 Claude Vision Hasar Tespiti
![DamageAssessment](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/DamageAssessment.png)
![DamageAssessment1](https://raw.githubusercontent.com/Cevdet-Karakulak/InsureYouAI/master/InsureYouAI/wwwroot/SS/DamageAssessment1.png)

---

## 👨‍💻 Geliştirici

**Cevdet Karakulak**  
🧩 Full Stack Developer  
🔗 LinkedIn: https://www.linkedin.com/in/cevdet/

---

## 🪪 Lisans

Bu proje **MIT Lisansı** ile paylaşılmıştır.
