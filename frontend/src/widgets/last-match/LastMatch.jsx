import LastMatchItem from '../../entities/last-match-item/LastMatchItem';

export default function LastMatch({ items }) {
  return (
    <div className="card" style={{gap: '16px'}}>
        <div className="header-row">
            <h3>Останні ігри</h3>
        </div>
        {items.map((item, index) => (
        <LastMatchItem
            key={index}
            firstOpponent={item.firstOpponent}
            secondOpponent={item.secondOpponent}
            result={item.result}
            sportName={item.sportName}
            typeColor={item.typeColor}
        />
        ))}
    </div>
  );
}

