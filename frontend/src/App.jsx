import React, { useState } from 'react';
import { Bot, FileText, Briefcase, ChevronRight, CheckCircle2, XCircle, AlertCircle, Loader2, Upload, FileUp } from 'lucide-react';
import { analyzeResume } from './services/resumeService';
import { extractTextFromFile } from './services/fileParser';

function App() {
  const [resumeText, setResumeText] = useState('');
  const [jobDescription, setJobDescription] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isParsing, setIsParsing] = useState(false);
  const [error, setError] = useState(null);
  const [result, setResult] = useState(null);
  const [fileName, setFileName] = useState('');

  const handleAnalyze = async () => {
    if (!resumeText || !jobDescription) {
      setError('Please provide both resume text and job description.');
      return;
    }
    if (resumeText.length < 50 || jobDescription.length < 30) {
      setError('Text provided is too short. Please provide meaningful content.');
      return;
    }

    setIsLoading(true);
    setError(null);
    setResult(null);

    try {
      const data = await analyzeResume(resumeText, jobDescription);
      setResult(data);
    } catch (err) {
      setError(err.message || 'Error connecting to the server. Is the API running?');
    } finally {
      setIsLoading(false);
    }
  };

  const handleFileUpload = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    setIsParsing(true);
    setError(null);
    setFileName(file.name);

    try {
      const text = await extractTextFromFile(file);
      if (!text || text.trim().length === 0) {
        throw new Error('Could not extract any text from the file. Please try another file or paste text manually.');
      }
      setResumeText(text);
    } catch (err) {
      setError(err.message || 'Error parsing file.');
      setFileName('');
    } finally {
      setIsParsing(false);
    }
  };

  const getScoreColor = (score) => {
    if (score >= 80) return 'high';
    if (score >= 50) return 'medium';
    return 'low';
  };

  return (
    <div className="app-container">
      <header className="header">
        <h1><Bot size={40} className="text-primary" /> AI Resume Analyzer</h1>
        <p>Optimize your resume for Applicant Tracking Systems in seconds</p>
      </header>

      <main className="main-content">
        <section className="glass-card">
          <h2 className="card-title"><FileText className="text-primary" /> Input Details</h2>
          
          {error && (
            <div className="error-message">
              <AlertCircle size={20} />
              <span>{error}</span>
            </div>
          )}

          <div className="form-group">
            <label htmlFor="jobDesc">Job Description</label>
            <textarea
              id="jobDesc"
              className="textarea-input"
              style={{ minHeight: '150px' }}
              placeholder="Paste the target job description here..."
              value={jobDescription}
              onChange={(e) => setJobDescription(e.target.value)}
            />
          </div>

          <div className="form-group">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
              <label htmlFor="resume" style={{ marginBottom: 0 }}>Resume Content</label>
              <div className="file-upload-wrapper">
                <input
                  type="file"
                  id="fileInput"
                  className="file-input-hidden"
                  accept=".pdf,.docx,.doc,.txt"
                  onChange={handleFileUpload}
                  style={{ display: 'none' }}
                />
                <button 
                  type="button" 
                  className="btn-secondary"
                  onClick={() => document.getElementById('fileInput').click()}
                  disabled={isParsing}
                  style={{ fontSize: '0.8rem', padding: '0.4rem 0.8rem' }}
                >
                  {isParsing ? (
                    <><Loader2 className="spinner" size={14} /> Parsing...</>
                  ) : (
                    <><FileUp size={14} /> Upload PDF/DOCX</>
                  )}
                </button>
              </div>
            </div>
            
            {fileName && (
              <div className="file-status">
                <CheckCircle2 size={14} className="text-success" />
                <span>Uploaded: <strong>{fileName}</strong></span>
                <button className="text-btn" onClick={() => { setFileName(''); setResumeText(''); }}>Clear</button>
              </div>
            )}

            <textarea
              id="resume"
              className="textarea-input"
              placeholder="Paste your resume text here or upload a file above..."
              value={resumeText}
              onChange={(e) => {
                setResumeText(e.target.value);
                if (fileName) setFileName('');
              }}
              style={{ minHeight: '200px' }}
            />
          </div>

          <button 
            className="btn-primary" 
            onClick={handleAnalyze} 
            disabled={isLoading || !resumeText || !jobDescription}
          >
            {isLoading ? (
              <><Loader2 className="spinner" size={20} /> Analyzing Match...</>
            ) : (
              <>Analyze Compatibility <ChevronRight size={20} /></>
            )}
          </button>
        </section>

        <section className="glass-card" style={{ display: result ? 'block' : 'none', opacity: result ? 1 : 0, transition: 'opacity 0.5s ease' }}>
          {result && (
            <>
              <h2 className="card-title text-center" style={{ justifyContent: 'center' }}>
                <Briefcase className="text-primary" /> Analysis Report
              </h2>

              <div className="score-circle" data-score={getScoreColor(result.atsScore)}>
                {result.atsScore}
              </div>
              <p style={{ textAlign: 'center', marginBottom: '2rem', color: 'var(--text-muted)' }}>
                {result.summary}
              </p>

              <div className="result-section">
                <h3>Matched Keywords</h3>
                <div className="tags-container">
                  {result.matchedKeywords?.map((kw, i) => (
                    <span key={i} className="tag success"><CheckCircle2 size={14} /> {kw}</span>
                  ))}
                  {!result.matchedKeywords?.length && <span className="text-muted">No keywords matched</span>}
                </div>
              </div>

              <div className="result-section">
                <h3>Missing Keywords</h3>
                <div className="tags-container">
                  {result.missingKeywords?.map((kw, i) => (
                    <span key={i} className="tag danger"><XCircle size={14} /> {kw}</span>
                  ))}
                  {!result.missingKeywords?.length && <span className="text-muted tag success">You matched all critical keywords!</span>}
                </div>
              </div>

              <div className="result-section flex-1">
                <h3>Actionable Suggestions</h3>
                <ul className="suggestions-list">
                  {result.suggestions?.map((suggestion, i) => (
                    <li key={i}>{suggestion}</li>
                  ))}
                  {!result.suggestions?.length && <li>Great job! Resume looks excellent.</li>}
                </ul>
              </div>
              
              <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', textAlign: 'center', marginTop: '2rem' }}>
                Model Used: {result.modelUsed}
              </div>
            </>
          )}
          
          {!result && !isLoading && (
            <div style={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', color: 'var(--text-muted)' }}>
              <Bot size={64} style={{ opacity: 0.2, marginBottom: '1rem' }} />
              <p>Your analysis report will appear here</p>
            </div>
          )}
        </section>
      </main>
    </div>
  );
}

export default App;
