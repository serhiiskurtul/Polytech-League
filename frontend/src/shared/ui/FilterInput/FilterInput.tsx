import styles from './FilterInput.module.css';

export default function FilterInput() {
  return (
    <input
        type="text"
        placeholder="Вид спорту"
        className={styles.filterInput}
    />
  );
}
