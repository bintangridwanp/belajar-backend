const multer = require('multer');
const path   = require('path');
const fs     = require('fs');

const UPLOAD_DIR = path.join(__dirname, '..', 'uploads');
if (!fs.existsSync(UPLOAD_DIR)) {
    fs.mkdirSync(UPLOAD_DIR, { recursive: true });
}


const storage = multer.diskStorage({
    destination: (_req, _file, cb) => {
        cb(null, UPLOAD_DIR);
    },
    filename: (_req, file, cb) => {
        const uniqueName = `${Date.now()}-${file.originalname}`;
        cb(null, uniqueName);
    },
});

const allowed = [
    // gambar
    'image/jpeg', 'image/png', 'image/gif',
    // audio
    'audio/mpeg', 'audio/wav', 'audio/x-wav', 'audio/mp4',
    // video
    'video/mp4', 'video/quicktime', 'video/x-matroska',
];

function fileFilter(_req, file, cb) {
    if (allowed.includes(file.mimetype)) {
        cb(null, true);
    } else {
        cb(new Error('Tipe file tidak diizinkan'), false);
    }
}

const MAX_SIZE_MB = 50;

const upload = multer({
    storage,
    fileFilter,
    limits: { fileSize: MAX_SIZE_MB * 1024 * 1024 },
});

module.exports = upload;
