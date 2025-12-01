import './LastMatchItem.css';

export default function LastMatchItem({ firstOpponent, secondOpponent, result }) {
  return (
    <section className="card-item match-item">
      <div className="match">
          <div className="team">{firstOpponent}</div>
          <div className="score">{result}</div>
          <div className="team">{secondOpponent}</div>
      </div>
    </section>
  );
}
