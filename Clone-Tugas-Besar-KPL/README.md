# 📘 Aplikasi Manajemen Tugas Mahasiswa - Tugas Besar KPL

## 📌 Deskripsi Proyek
Aplikasi ini dirancang untuk membantu mahasiswa dalam mencatat, mengelola, dan memantau progres tugas akademik maupun non-akademik seperti tugas kuliah, proyek kelompok, dan kegiatan organisasi. Proyek ini dikerjakan oleh tim beranggotakan 5 orang dalam konteks tugas besar mata kuliah **Konstruksi Perangkat Lunak (KPL)**.

Aplikasi dibangun menggunakan **C# dengan Visual Studio**, memanfaatkan pendekatan arsitektur modular agar mudah dikembangkan dan dipelihara.

---

## ⚙️ Fitur Utama
- CRUD tugas: tambah, ubah, dan hapus data tugas
- Filter tugas berdasarkan deadline, status, dan kategori
- Reminder deadline visual
- Statistik jumlah tugas selesai, belum, dan overdue
- Mapping status ke prioritas/warna
- Export laporan ke format PDF/CSV
- Konfigurasi runtime (prioritas default, file config)
- Dukungan CLI untuk integrasi otomatis
- Dokumentasi internal dengan XML Comments

---

## 🧩 Arsitektur Komponen

Aplikasi menggunakan pendekatan arsitektur **Model-View-Presenter (MVP)** dengan pemisahan tanggung jawab sebagai berikut:

### 🔹 Frontend
- **View**: Menyediakan antarmuka berbasis terminal, menangani input pengguna dan output hasil. Tidak menyimpan logika aplikasi.
- **Presenter**: Mengatur komunikasi antara View dan Backend, memproses hasil, dan memformat data agar siap ditampilkan.

### 🔹 Backend
- **Model**: Representasi data seperti `Tugas`, `Konfigurasi`, dan `Statistik`. Mendukung validasi status, parameterisasi, dan table-driven logic.
- **Service Layer**: Menyediakan layanan seperti `TaskService`, `ConfigService`, dan `StatistikService` untuk memproses logika aplikasi secara terpisah dari tampilan.
- **Repository**: Interface dan implementasi untuk manipulasi data tugas menggunakan pendekatan `List<T>` atau SQLite.

### 🔹 API Internal
- Bertindak sebagai jembatan antara Presenter dan Service, memungkinkan fleksibilitas, injeksi dependensi, dan modularitas.

📄 Diagram dan deskripsi lengkap tersedia di [`/docs/arsitektur.md`](./docs/arsitektur.md)

---

## 🗃️ Struktur Proyek

Struktur folder mengacu pada praktik terbaik dalam pengembangan perangkat lunak berbasis C#:


📁 Referensi: [`/docs/Sturuktur_Folder.md`](./docs/Sturuktur_Folder.md)&#8203;

---

## 🧠 Pembagian Tugas Anggota

| Anggota | Tugas Utama                        | Teknik Konstruksi                      | Keterangan                                                                 |
|---------|------------------------------------|----------------------------------------|----------------------------------------------------------------------------|
| **Zuhri**       | Modul CRUD Tugas                   | Automata, Code Reuse/Library           | Validasi status, helper reusable (InputValidator, DateHelper)             |
| **Aryo**      | Filtering dan Statistik            | Table-driven, Parameterization         | Tabel mapping + filter generik List<T>                                    |
| **Zhafran**       | Modul Konfigurasi Aplikasi         | Runtime Configuration, API     | Baca config.json (default notifikasi, format tanggal), reusable config    |
| **Bintang**      | Defensive Programming + Logging    | Code Reuse, Automata     | Validasi input (pre/postcondition), pengecekan state, logging handler     |
| **Rifki**    | Unit & Performance Testing         | Parameterization, API                  | Unit test TaskService + performa filter + interface API internal TaskService |

---

## 🗄️ Skema Basis Data

Skema database dirancang menggunakan SQLite dan digambarkan dengan **PlantUML**. Terdiri dari tabel berikut:

- `Tugas` — Menyimpan detail tugas
- `Konfigurasi` — Runtime setting
- `Prioritas` — Mapping status ke warna/prioritas
- `Statistik` — Ringkasan progres tugas

📄 Diagram visual tersedia di: [`/docs/skema_database.plantuml`](./docs/skema_database.plantuml)

---

## 🧪 Teknologi & Tools

- **Bahasa**: C# (.NET)
- **IDE**: Visual Studio 2022
- **Database**: SQLite
- **Testing**: MSTest / xUnit
- **Ekspor File**: CSV, PDF
- **Dokumentasi**: Markdown
- **Visualisasi Skema**: PlantUML

---

## ✍️ Kontributor

Disusun dan dikembangkan oleh Tim Mahasiswa RPL 2023
Universitas Telkom | Fakultas Informatika 

---
