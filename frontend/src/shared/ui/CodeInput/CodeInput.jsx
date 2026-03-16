import React, { useRef, useEffect } from 'react';
import styles from './CodeInput.module.css';

const CodeInput = ({ code, onChange, hasError, autoFocus = false }) => {
  const inputsRef = useRef([]);
  
  useEffect(() => {
    inputsRef.current = inputsRef.current.slice(0, 6);
  }, []);

  const handleChange = (index, value) => {
    if (value && !/^\d$/.test(value)) {
      return;
    }
    
    const newCode = [...code];
    newCode[index] = value;
    onChange(newCode);
    
    if (value && index < 5) {
      inputsRef.current[index + 1].focus();
    }
  };

  const handleKeyDown = (index, e) => {
    if (e.key === 'Backspace') {
      if (!code[index] && index > 0) {
        const newCode = [...code];
        newCode[index - 1] = '';
        onChange(newCode);
        inputsRef.current[index - 1].focus();
      }
    } else if (e.key === 'ArrowLeft' && index > 0) {
      inputsRef.current[index - 1].focus();
    } else if (e.key === 'ArrowRight' && index < 5) {
      inputsRef.current[index + 1].focus();
    }
  };

  const handlePaste = (e) => {
    e.preventDefault();
    const pastedData = e.clipboardData.getData('text');
    
    if (pastedData.length === 6 && /^\d+$/.test(pastedData)) {
      const newCode = pastedData.split('');
      onChange(newCode);
      
      inputsRef.current[5].focus();
    }
  };

  return (
    <div className={styles.codeContainer}>
      {code.map((digit, index) => (
        <input
          key={index}
          ref={el => inputsRef.current[index] = el}
          type="text"
          className={`${styles.codeInput} ${digit ? styles.filled : ''} ${hasError ? styles.error : ''}`}
          maxLength="1"
          value={digit}
          onChange={(e) => handleChange(index, e.target.value)}
          onKeyDown={(e) => handleKeyDown(index, e)}
          onPaste={index === 0 ? handlePaste : null}
          inputMode="numeric"
          autoFocus={autoFocus && index === 0}
        />
      ))}
    </div>
  );
};

export default CodeInput;