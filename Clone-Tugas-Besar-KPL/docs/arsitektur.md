# Arsitektur Aplikasi Manajemen Tugas Mahasiswa

## 1. Tujuan Desain
Merancang arsitektur modular dan skalabel untuk aplikasi manajemen tugas mahasiswa berbasis C#, yang memungkinkan pengelolaan tugas akademik dan non-akademik secara efisien oleh pengguna. Aplikasi ini dibangun dengan memperhatikan alur kerja tim sebanyak 5 orang, serta mendukung ekspansi dan pemeliharaan jangka panjang.

---

## 2. Komponen Utama

### 2.1. Frontend
- **View**:
  - Menyediakan tampilan input/output melalui terminal (CLI)
  - Menangani antarmuka pengguna: input tugas, filter, update status, ekspor data
  - Tidak memiliki logika bisnis langsung, hanya sebagai perantara pengguna dan presenter

- **Presenter**:
  - Menangani alur presentasi data (status validasi, hasil filter, reminder)
  - Menghubungkan View dan Model tanpa ketergantungan langsung
  - Memanggil layanan di backend dan memformat hasil ke bentuk yang bisa ditampilkan

### 2.2. Backend (Model dan Controller)

- **Model**:
  - Representasi data seperti `Tugas`, `Konfigurasi`, dan `Statistik`
  - Termasuk automata validasi status (To Do → Doing → Done)
  - Mendukung parameterisasi (List<T>) dan table-driven logic

- **Controller / Service Layer**:
  - Berfungsi sebagai "business service" dalam MVP
  - Menyediakan layanan seperti: `TaskService`, `ConfigService`, dan `StatistikService`
  - Bertanggung jawab atas: CRUD data, validasi input (design by contract), filter tugas, logging error

- **Repository**:
  - `ITugasRepository` dan `TugasRepository` sebagai interface dan implementasi manipulasi data
  - Simulasi database dengan `List<T>` atau implementasi nyata dengan SQLite

### 2.3. API Internal (untuk komunikasi Presenter ↔ Controller)
- Digunakan untuk memisahkan logika presentasi dari logika bisnis
- Mendukung parameterisasi, dependensi injeksi, dan modularitas

---

## 3. Alur Data dan Komunikasi

1. **User** menginput data ke View
2. View meneruskan input ke Presenter
3. Presenter memanggil `TaskService` atau `ConfigService` di Backend
4. Service melakukan validasi, transformasi, dan menyimpan/mengambil data via Repository
5. Hasil akhir diformat oleh Presenter dan dikirim kembali ke View untuk ditampilkan

---

## 4. Skalabilitas
- Modularisasi memudahkan ekspansi ke versi GUI/Web
- MVP memisahkan logika bisnis dari antarmuka pengguna (CLI)
- Reusability tinggi lewat service dan helper reusable
- Logging dan Defensive Programming membantu debugging dan reliability

---

## 5. Dokumentasi
- Semua komponen menggunakan XML documentation comments
- Penamaan dan struktur mengikuti standar .NET
- Diagram arsitektur, skema database, dan manual CLI berada di direktori `/docs`

---

Disusun oleh: Tim kelompok 5 (Zuhri, Aryo, Zhafran, Bintang, Rifki)

