import LastMatchItem from './LastMatchItem/LastMatchItem';
import './LastMatch.css';

export default function LastMatch({ items }) {
  return (
    <div className="card">
        <div className="header-row">
            <h3>Останні матчі</h3>
        </div>
        {items.map((item, index) => (
        <LastMatchItem
            key={index}
            firstOpponent={item.firstOpponent}
            secondOpponent={item.secondOpponent}
            result={item.result}
        />
        ))}
    </div>
  );
}

