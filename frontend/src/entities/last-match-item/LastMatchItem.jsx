import './LastMatchItem.css';

export default function LastMatchItem({ firstOpponent, secondOpponent, result, sportName, typeColor }) {
  return (
    <section className="card-item match-item">
      <div className={`match ${typeColor} standard-hover-animation`}>
          <div className="team">{firstOpponent}</div>
          <div className="match-info">
            <div className="result">{result}</div>
            <div className="sportName">{sportName}</div>
          </div>   
          <div className="team">{secondOpponent}</div>
      </div>
    </section>
  );
}
