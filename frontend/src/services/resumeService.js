import apiClient from './apiClient';

export const analyzeResume = async (resumeText, jobDescription) => {
  return apiClient('/ResumeAnalyzer/Analyze', {
    method: 'POST',
    body: {
      resumeText,
      jobDescription,
    },
  });
};
