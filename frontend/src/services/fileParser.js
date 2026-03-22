import * as pdfjsLib from 'pdfjs-dist';
import pdfWorker from 'pdfjs-dist/build/pdf.worker?url';
import mammoth from 'mammoth';
import Tesseract from 'tesseract.js';

pdfjsLib.GlobalWorkerOptions.workerSrc = pdfWorker;

/**
 * Main entry
 */
export const extractTextFromFile = async (file) => {
  const extension = file.name.split('.').pop().toLowerCase();

  switch (extension) {
    case 'pdf':
      return await extractTextFromPDF(file);

    case 'doc':
    case 'docx':
      return await extractTextFromDocx(file);

    case 'txt':
      return await extractTextFromTxt(file);

    default:
      throw new Error('Unsupported file format');
  }
};

/**
 * PDF Extraction
 */
const extractTextFromPDF = async (file) => {
  const arrayBuffer = await file.arrayBuffer();
  const pdf = await pdfjsLib.getDocument({ data: arrayBuffer }).promise;

  let text = '';

  for (let i = 1; i <= pdf.numPages; i++) {
    const page = await pdf.getPage(i);
    const content = await page.getTextContent();

    const strings = content.items.map((item) => item.str);
    text += strings.join(' ') + '\n';
  }

  // OCR fallback
  if (!text.trim()) {
    return await runOCRonPDF(pdf);
  }

  return cleanText(text);
};

/**
 * OCR fallback (PDF → Image → Text)
 */
const runOCRonPDF = async (pdf) => {
  let finalText = '';

  for (let i = 1; i <= pdf.numPages; i++) {
    const page = await pdf.getPage(i);

    const viewport = page.getViewport({ scale: 2 });

    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');

    canvas.width = viewport.width;
    canvas.height = viewport.height;

    await page.render({
      canvasContext: context,
      viewport,
    }).promise;

    const image = canvas.toDataURL('image/png');

    const result = await Tesseract.recognize(image, 'eng', {
      // logger: (m) => console.log(m),
    });

    finalText += result.data.text + '\n';
  }

  return cleanText(finalText);
};

/**
 * DOCX Extraction
 */
const extractTextFromDocx = async (file) => {
  const arrayBuffer = await file.arrayBuffer();
  const result = await mammoth.extractRawText({ arrayBuffer });

  return cleanText(result.value);
};

/**
 * TXT Extraction
 */
const extractTextFromTxt = (file) => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();

    reader.onload = (e) => resolve(cleanText(e.target.result));
    reader.onerror = () => reject(new Error('TXT read error'));

    reader.readAsText(file);
  });
};

/**
 * Clean text
 */
const cleanText = (text) => {
  return text
    .replace(/\r/g, '')
    .replace(/\t/g, ' ')
    .replace(/\s+/g, ' ')
    .replace(/\n\s*\n/g, '\n')
    .trim();
};