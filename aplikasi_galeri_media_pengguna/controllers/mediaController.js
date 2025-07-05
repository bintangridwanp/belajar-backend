// controllers/mediaController.js
const mediaModel = require('../models/mediaModel');

exports.uploadMedia = async (req, res) => {
    try {
        const file = req.file;
        if (!file) return res.status(400).json({ error: 'No file uploaded' });

        const saved = await mediaModel.create({
            filename: file.originalname,
            filepath: file.path,
            mimetype: file.mimetype,
            size: file.size,
        });

        res.status(201).json(saved);
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
};

exports.getAllMedia = async (req, res) => {
    try {
        const files = await mediaModel.findAll();
        res.json(files);
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
};

exports.getMediaById = async (req, res) => {
    try {
        const file = await mediaModel.findById(req.params.id);
        if (!file) return res.status(404).json({ error: 'Media not found' });
        res.json(file);
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
};

exports.deleteMedia = async (req, res) => {
    try {
        const deleted = await mediaModel.remove(req.params.id);
        res.json(deleted);
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
};