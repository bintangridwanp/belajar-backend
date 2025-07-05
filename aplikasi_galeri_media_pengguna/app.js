const express    = require('express');
const mediaRoutes = require('./routes/mediaRoutes');

const app = express();
app.use(express.json());
app.use('/api', mediaRoutes); // prefix opsional

app.listen(process.env.PORT || 3000, () =>
    console.log('Server ready 🚀'),
);
