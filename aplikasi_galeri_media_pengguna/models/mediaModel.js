const prisma = require('../prisma/client');

module.exports = {

    async create(fileData) {
        return prisma.media.create({ data: fileData });
    },


    async findAll() {
        return prisma.media.findMany({
            orderBy: { uploadedAt: 'desc' },
        });
    },

    async findById(id) {
        return prisma.media.findUnique({
            where: { id: Number(id) },
        });
    },


    async remove(id) {
        return prisma.media.delete({
            where: { id: Number(id) },
        });
    },
};
