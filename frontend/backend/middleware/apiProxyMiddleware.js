export function apiProxyMiddleware(req, res, next) {
  const targetBase = process.env.TARGET_API_BASE;

  if (!targetBase) {
    return res.status(500).json({
      error: 'TARGET_API_BASE is not configured',
      message: 'Please set TARGET_API_BASE in backend/.env or your environment variables.',
    });
  }

  const originalUrl = req.originalUrl || req.url;
  const proxyPath = originalUrl.startsWith('/api') ? originalUrl.slice(4) : originalUrl;
  req.targetUrl = `${targetBase}${proxyPath || '/'}`;

  next();
}
