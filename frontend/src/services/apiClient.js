const BASE_URL = import.meta.env.VITE_API_BASE_URL;
const endpoint = import.meta.env._endpoint || '/redirectApiRequest';


const apiClient = async (options = {}) => {
  const { method = 'POST', body, headers = {} } = options;
  const response = await fetch(`${BASE_URL}${endpoint}`, {
    method,
    headers: {
      'Content-Type': 'application/json',
      'accept': 'application/json',
      ...headers,
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  // Handle potential empty responses
  const contentType = response.headers.get('content-type');
  let data = {};
  if (contentType && contentType.includes('application/json')) {
    data = await response.json();
  }

  if (!response.ok) {
    throw new Error(data.detail || data.title || `API Error: ${response.statusText}`);
  }

  return data;
};

export default apiClient;
