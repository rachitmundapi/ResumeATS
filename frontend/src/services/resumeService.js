import apiClient from './apiClient';

export const analyzeResume = async (resumeText, jobDescription) => {
  return apiClient({
    method: 'POST',
    body: {
      resumeText,
      jobDescription,
    },
  });
};
