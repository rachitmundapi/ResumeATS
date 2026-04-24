const targetUrl = process.env.TARGET_API_BASE;
export async function redirectApiRequest(req, res, next) {
  try {
    const headers = { ...req.headers };
    delete headers.host;

    const fetchOptions = {
      method: req.method,
      headers,
      body: ['GET', 'HEAD'].includes(req.method) ? undefined : JSON.stringify(req.body),
    };

    const response = await fetch(targetUrl, fetchOptions);
    res.status(response.status);

    response.headers.forEach((value, name) => {
      if (!['content-encoding', 'transfer-encoding', 'connection'].includes(name.toLowerCase())) {
        res.setHeader(name, value);
      }
    });

    const buffer = await response.arrayBuffer();
    res.send(Buffer.from(buffer));
  } catch (error) {
    next(error);
  }
}
