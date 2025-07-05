// routes/mediaRoutes.js
const express = require('express');
const router  = express.Router();

const upload        = require('../config/multer');            // middleware upload
const mediaCtrl = require('../controllers/mediaController');

router.post('/upload', upload.single('file'), mediaCtrl.uploadMedia);
router.get('/files', mediaCtrl.getAllMedia);
router.get('/files/:id', mediaCtrl.getMediaById);
router.delete('/files/:id', mediaCtrl.deleteMedia);

module.exports = router;
