# Struktur Proyek Aplikasi Manajemen Tugas Mahasiswa

## Struktur Folder
```
project-root/
│
├── /src/                                # Kode sumber utama
│   ├── /Presentation/                   # Lapisan View & Presenter (Frontend/CLI)
│   │   ├── /Views/                      # Tampilan CLI
│   │   └── /Presenters/                 # Presenter (handle alur dan komunikasi service)
│   │
│   ├── /Domain/                         # Model dan kontrak (Domain Layer)
│   │   ├── /Models/                     # Entity seperti Tugas, Konfigurasi, dll
│   │   └── /Interfaces/                 # Interface untuk Repository dan Service
│   │
│   ├── /Application/                    # Business logic layer (Controller/Service)
│   │   ├── /Services/                   # TaskService, ConfigService, StatistikService
│   │   └── /Helpers/                    # InputValidator, DateHelper, MappingTable
│   │
│   └── /Infrastructure/                 # Implementasi repository & konfigurasi runtime
│       ├── /Repositories/               # Implementasi SQLite/List<T> dari ITugasRepository
│       └── /Configuration/              # Config handler (JSON/XML)
│   
│
│
├── /data/                               # Data dan export
│   ├── database.db                      # SQLite file
│   └── /exports/                        # File PDF, CSV
│
├── /tests/                              # Unit dan performance test
│   ├── /UnitTests/                      # Pengujian unit service dan presenter
│   └── /Performance/                    # Pengujian performa filtering/statistik
│
├── /cli/                                # CLI tooling (opsional, bisa jadi executable sendiri)
│
├── /docs/                               # Dokumentasi proyek
│   ├── arsitektur.md
│   ├── skema_database.plantuml
│   ├── Struktur_Folder.md
│   └── 
│
├── TaskManager.sln                      # File solusi Visual Studio
├── README.md
└── Program.cs                           # Entry point
```

## Deskripsi Singkat Komponen

### 📁 `/src/`
Kode sumber utama aplikasi dengan pemisahan tanggung jawab berdasarkan arsitektur MVP.

#### `/Presentation/Views/`
- Tampilan antarmuka CLI.
- Menampilkan data dan menerima input pengguna.

#### `/Presentation/Presenters/`
- Mengatur logika presentasi dan menghubungkan View ↔ Service.

#### `/Domain/Models/`
- Berisi entity/domain class seperti `Tugas`, `Konfigurasi`, dan `Statistik`.

#### `/Domain/Interfaces/`
- Berisi antarmuka (interface) untuk repository dan service.

#### `/Application/Services/`
- Implementasi business logic utama aplikasi seperti `TaskService`, `ConfigService`, dll.

#### `/Application/Helpers/`
- Kelas utilitas seperti validasi input, pemetaan status, dsb.

#### `/Infrastructure/Repositories/`
- Implementasi penyimpanan data (`List<T>`, SQLite) berdasarkan interface.

#### `/Infrastructure/Configuration/`
- Handler untuk konfigurasi runtime berbasis file JSON/XML.

#### `Program.cs`
- Titik awal (entry point) untuk menjalankan aplikasi CLI.

### 📁 `/data/`
- Menyimpan file database dan hasil ekspor laporan.

### 📁 `/tests/`
- Unit test dan pengujian performa aplikasi.

### 📁 `/cli/`
- Tool tambahan berbasis CLI (opsional).

### 📁 `/docs/`
- Dokumentasi teknis dan desain arsitektur proyek.

### `TaskManager.sln`
- Solusi utama untuk membuka proyek di Visual Studio.

## Diagram Skema
Lihat file `skema_database.plantuml` untuk relasi antar tabel, seperti `Tugas`, `Konfigurasi`, `Statistik`, dan `Prioritas`.

---
Disusun oleh: Tim kelompok 5 (Zuhri, Aryo, Zhafran, Bintang, Rifki)

