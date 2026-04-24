import 'dotenv/config';
import express from 'express';
import { apiProxyMiddleware } from './middleware/apiProxyMiddleware.js';
import { redirectApiRequest } from './controllers/apiController.js';
import cors from 'cors';

const app = express();
const PORT = process.env.PORT ? Number(process.env.PORT) : 4000;

app.use(cors());

app.use(express.json());
app.use('/api', apiProxyMiddleware, redirectApiRequest);

app.use((err, req, res, next) => {
  console.error('[API Proxy Error]', err);
  res.status(500).json({
    error: 'Proxy error',
    message: err.message || 'Unexpected server error',
  });
});

app.listen(PORT, () => {
  console.log(`Express redirect server listening on ${PORT}`);
});
