import React from 'react';
import UniversalBracket, { BracketRound } from './UniversalBracket';

const sampleData: BracketRound[] = [
  {
    title: "1/4 Фіналу",
    matches: [
      {
        id: "m1",
        teams: [
          { name: "Команда 511", score: 0 },
          { name: "Команда 512", score: 1, isWinner: true }
        ]
      },
      {
        id: "m2",
        teams: [
          { name: "Команда 513", score: 0 },
          { name: "Команда 521", score: 1, isWinner: true }
        ]
      },
      {
        id: "m3",
        teams: [
          { name: "Команда 522", score: 0 },
          { name: "Команда 523", score: 1, isWinner: true }
        ]
      },
      {
        id: "m4",
        teams: [
          { name: "Команда 531", score: 0 },
          { name: "Команда 532", score: 1, isWinner: true }
        ]
      }
    ]
  },
  {
    title: "Півфінал",
    matches: [
      {
        id: "m5",
        teams: [
          { name: "Команда 512", score: 2, isWinner: true },
          { name: "Команда 521", score: 1 }
        ]
      },
      {
        id: "m6",
        teams: [
          { name: "Команда 523", score: "-" },
          { name: "Команда 532", score: "-" }
        ]
      }
    ]
  },
  {
    title: "Фінал",
    matches: [
      {
        id: "m7",
        teams: [
          { name: "Команда 512", score: "-" },
          { name: "TBD", score: "-" }
        ]
      }
    ]
  }
];

const UniversalBracketDemo: React.FC = () => {
  return (
    <div style={{ padding: '40px', background: '#f1f5f9', minHeight: '100vh' }}>
      <h1 style={{ marginBottom: '40px', textAlign: 'center', color: '#1e293b' }}>
        Tournament Bracket Demo
      </h1>
      <UniversalBracket rounds={sampleData} />
    </div>
  );
};

export default UniversalBracketDemo;
