import shared from '../form.module.css';
import styles from './SettingsSection.module.css';
import { 
  CHECKERS_VARIANTS, 
  TABLE_TENNIS_CATEGORIES, 
  ARMWRESTLING_HANDS, 
  ARMWRESTLING_WEIGHT_CLASSES, 
  ARMWRESTLING_CATEGORIES 
} from '../../model/constants';
import { TournamentFormData } from '../../model/useTournamentForm';
import { ChangeEvent } from 'react';

interface SettingsSectionProps {
  formData: TournamentFormData;
  handleInputChange: (e: ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => void;
}

const SettingsSection = ({ formData, handleInputChange }: SettingsSectionProps) => {
  return (
    <>
      <div className={shared.formBlock}>
        <h3 className={shared.blockTitle}>
          <i className="fas fa-sliders-h"></i> Додаткові налаштування
        </h3>
        <div className={shared.formGrid}>
          {/* Рівень змагання */}
          <div className={shared.formGroup}>
            <label className={shared.formLabel}>
              <i className="fas fa-layer-group"></i> Рівень змагання *
            </label>
            <select
              name="level"
              value={formData.level || "CollegeGroup"}
              onChange={handleInputChange}
              className={shared.formSelect}
              required
            >
              <option value="CollegeGroup">Міжгрупові (коледж)</option>
              <option value="Regional">Регіональні</option>
              <option value="National">Всеукраїнські</option>
            </select>
          </div>

          {/* Варіант шашок та налаштування для шашок/шахів */}
          {(formData.sport === 'chess' || formData.sport === 'checkers') && (
            <>
              <div className={shared.formGroup}>
                <label className={shared.formLabel}>
                  <i className="fas fa-stopwatch"></i> Контроль часу *
                </label>
                <input
                  type="text"
                  name="timeControl"
                  value={formData.timeControl}
                  onChange={handleInputChange}
                  placeholder="напр. 10+5 або 10хв"
                  className={shared.formInput}
                  required
                />
              </div>

              <div className={shared.formGroup}>
                <label className={shared.formLabel}>
                  <i className="fas fa-project-diagram"></i> Система проведення *
                </label>
                <select
                  name="system"
                  value={formData.system}
                  onChange={handleInputChange}
                  className={shared.formSelect}
                  required
                >
                  <option value="Swiss">Швейцарська система</option>
                  <option value="RoundRobin">Кругова система</option>
                  <option value="Knockout">Олімпійська система (Knockout)</option>
                </select>
              </div>

              <div className={shared.formGroup} style={{ gridColumn: '1 / -1' }}>
                <label className={shared.checkboxLabel}>
                  <div className={shared.checkboxWrapper}>
                    <input
                      type="checkbox"
                      name="isTeam"
                      checked={formData.isTeam}
                      onChange={handleInputChange}
                      className={shared.hiddenCheckbox}
                    />
                    <div className={shared.customCheckbox}>
                      <i className="fas fa-check"></i>
                    </div>
                  </div>
                  <div className={shared.checkboxText}>
                    <span className={shared.checkboxTitle}>Командні змагання</span>
                    <span className={shared.checkboxDesc}>Змагання проводитимуться між командами, а не індивідуальними гравцями</span>
                  </div>
                </label>
              </div>

              {formData.sport === 'checkers' && (
                <div className={shared.formGroup}>
                  <label className={shared.formLabel}>
                    <i className="fas fa-th-large"></i> Варіант шашок *
                  </label>
                  <select
                    name="variant"
                    value={formData.variant || "International"}
                    onChange={handleInputChange}
                    className={shared.formSelect}
                    required
                  >
                    {CHECKERS_VARIANTS.map(variant => (
                      <option key={variant.id} value={variant.id}>
                        {variant.name}
                      </option>
                    ))}
                  </select>
                </div>
              )}
            </>
          )}

          {/* Налаштування настільного тенісу */}
          {formData.sport === 'table_tennis' && (
            <div className={shared.formGroup}>
              <label className={shared.formLabel}>
                <i className="fas fa-users"></i> Категорія *
              </label>
              <select
                name="category"
                value={formData.category || "Men"}
                onChange={handleInputChange}
                className={shared.formSelect}
                required
              >
                {TABLE_TENNIS_CATEGORIES.map(cat => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* Налаштування армреслінгу */}
          {formData.sport === 'armwrestling' && (
            <>
              <div className={shared.formGroup}>
                <label className={shared.formLabel}>
                  <i className="fas fa-hand-paper"></i> Рука *
                </label>
                <select
                  name="armHand"
                  value={formData.armHand || 'Right'}
                  onChange={handleInputChange}
                  className={shared.formSelect}
                  required
                >
                  {ARMWRESTLING_HANDS.map(h => (
                    <option key={h.id} value={h.id}>{h.name}</option>
                  ))}
                </select>
              </div>

              <div className={shared.formGroup}>
                <label className={shared.formLabel}>
                  <i className="fas fa-weight-hanging"></i> Вагова категорія *
                </label>
                <select
                  name="armWeightClass"
                  value={formData.armWeightClass || 'Under80kg'}
                  onChange={handleInputChange}
                  className={shared.formSelect}
                  required
                >
                  {ARMWRESTLING_WEIGHT_CLASSES.map(w => (
                    <option key={w.id} value={w.id}>{w.name}</option>
                  ))}
                </select>
              </div>

              <div className={shared.formGroup}>
                <label className={shared.formLabel}>
                  <i className="fas fa-users"></i> Категорія *
                </label>
                <select
                  name="armCategory"
                  value={formData.armCategory || 'Men'}
                  onChange={handleInputChange}
                  className={shared.formSelect}
                  required
                >
                  {ARMWRESTLING_CATEGORIES.map(c => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
              </div>
            </>
          )}
        </div>
      </div>

      {/* Налаштування видимості */}
      <div className={shared.formBlock}>
        <h3 className={shared.blockTitle}>
          <i className="fas fa-cog"></i> Налаштування доступу
        </h3>
        <div className={shared.formGrid}>
          <div className={shared.checkboxGroup}>
            <label className={shared.checkboxLabel}>
              <div className={shared.checkboxWrapper}>
                <input
                  type="checkbox"
                  name="isPublic"
                  checked={formData.isPublic}
                  onChange={handleInputChange}
                  className={shared.hiddenCheckbox}
                />
                <div className={shared.customCheckbox}>
                  <i className="fas fa-check"></i>
                </div>
              </div>
              <div className={shared.checkboxText}>
                <span className={shared.checkboxTitle}>Публічне змагання</span>
                <span className={shared.checkboxDesc}>Змагання будуть видимими для всіх користувачів</span>
              </div>
            </label>
          </div>
          <div className={shared.checkboxGroup}>
            <label className={shared.checkboxLabel}>
              <div className={shared.checkboxWrapper}>
                <input
                  type="checkbox"
                  name="requireApproval"
                  checked={formData.requireApproval}
                  onChange={handleInputChange}
                  className={shared.hiddenCheckbox}
                />
                <div className={shared.customCheckbox}>
                  <i className="fas fa-check"></i>
                </div>
              </div>
              <div className={shared.checkboxText}>
                <span className={shared.checkboxTitle}>Підтвердження реєстрації</span>
                <span className={shared.checkboxDesc}>Організатор повинен схвалити кожну заявку</span>
              </div>
            </label>
          </div>
        </div>
      </div>
    </>
  );
};

export default SettingsSection;
