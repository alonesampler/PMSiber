import { useEffect, useState, useCallback } from "react";
import { EmployeesApi } from "../../../api/employees.api";
import type { ProjectFormData } from "../../../types/project.types";
import type { EmployeeResponseDto } from "../../../types/employee.types";

type Props = {
  formData: ProjectFormData;
  onChange: (data: ProjectFormData) => void;
};

const ProjectEditForm = ({ formData, onChange }: Props) => {
  const [managerQuery, setManagerQuery] = useState("");
  const [managerResults, setManagerResults] = useState<EmployeeResponseDto[]>([]);
  const [managerLoading, setManagerLoading] = useState(false);
  const [selectedManager, setSelectedManager] = useState<EmployeeResponseDto | null>(null);

  const [teamQuery, setTeamQuery] = useState("");
  const [allEmployees, setAllEmployees] = useState<EmployeeResponseDto[]>([]);
  const [teamFiltered, setTeamFiltered] = useState<EmployeeResponseDto[]>([]);
  const [teamLoading, setTeamLoading] = useState(false);

  useEffect(() => {
    loadAllEmployees();
  }, []);

  useEffect(() => {
    if (formData.managerId && allEmployees.length > 0) {
      const manager = allEmployees.find(e => e.id === formData.managerId);
      setSelectedManager(manager || null);
    } else {
      setSelectedManager(null);
    }
  }, [formData.managerId, allEmployees]);

  useEffect(() => {
    if (!teamQuery.trim()) {
      setTeamFiltered(allEmployees.filter(emp => emp.id !== formData.managerId));
    } else {
      const query = teamQuery.toLowerCase();
      const filtered = allEmployees.filter(emp => {
        const fullName = `${emp.lastName} ${emp.firstName} ${emp.middleName || ''}`.toLowerCase();
        const matchesSearch = fullName.includes(query) || emp.email.toLowerCase().includes(query);
        const notManager = emp.id !== formData.managerId;
        return matchesSearch && notManager;
      });
      setTeamFiltered(filtered);
    }
  }, [teamQuery, allEmployees, formData.managerId]);

  const loadAllEmployees = async () => {
    setTeamLoading(true);
    try {
      const allEmployees = await EmployeesApi.getAll();
      setAllEmployees(allEmployees);
      setTeamFiltered(allEmployees.filter(emp => emp.id !== formData.managerId));
    } catch (error) {
      console.error("Failed to load employees:", error);
      alert("Ошибка при загрузке сотрудников");
    } finally {
      setTeamLoading(false);
    }
  };

  const handleSearchManager = useCallback(async (searchQuery: string) => {
    setManagerQuery(searchQuery);
    
    if (!searchQuery.trim()) {
      setManagerResults([]);
      return;
    }

    setManagerLoading(true);
    try {
      const results = await EmployeesApi.search(searchQuery);
      const filteredResults = results.filter(emp => emp.id !== formData.managerId);
      setManagerResults(filteredResults);
    } catch (error) {
      console.error("Search failed:", error);
    } finally {
      setManagerLoading(false);
    }
  }, [formData.managerId]);

  const handleSelectManager = (employee: EmployeeResponseDto) => {
    onChange({ ...formData, managerId: employee.id });
    
    const newEmployeesIds = formData.employeesIds.filter(id => id !== employee.id);
    onChange({ ...formData, managerId: employee.id, employeesIds: newEmployeesIds });
    
    setManagerQuery("");
    setManagerResults([]);
  };

  const handleToggleEmployee = (employeeId: string) => {
    const isSelected = formData.employeesIds.includes(employeeId);
    const newEmployeesIds = isSelected
      ? formData.employeesIds.filter(id => id !== employeeId)
      : [...formData.employeesIds, employeeId];
    
    onChange({ ...formData, employeesIds: newEmployeesIds });
  };

  const formatEmployeeName = (employee: EmployeeResponseDto) => {
    return `${employee.lastName} ${employee.firstName}${employee.middleName ? ` ${employee.middleName}` : ''}`;
  };

  const clearTeam = () => {
    onChange({ ...formData, employeesIds: [] });
  };

  return (
    <div className="space-y-6">
      <div className="edit-form-section">
        <h2 className="h2">Основная информация</h2>
        
        <div className="space-y-6">
          <div className="form-field">
            <label className="required-field">Название проекта</label>
            <input
              type="text"
              className="input"
              placeholder="Введите название проекта"
              value={formData.params.name}
              onChange={e => onChange({
                ...formData,
                params: { ...formData.params, name: e.target.value }
              })}
              required
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="form-field">
              <label className="required-field">Дата начала</label>
              <input
                type="date"
                className="input"
                value={formData.params.startDate}
                onChange={e => onChange({
                  ...formData,
                  params: { ...formData.params, startDate: e.target.value }
                })}
                required
              />
            </div>
            
            <div className="form-field">
              <label className="required-field">Дата окончания</label>
              <input
                type="date"
                className="input"
                value={formData.params.endDate}
                min={formData.params.startDate}
                onChange={e => onChange({
                  ...formData,
                  params: { ...formData.params, endDate: e.target.value }
                })}
                required
              />
            </div>
          </div>

          <div className="form-field">
            <label>
              Приоритет: <span className="font-bold">{formData.params.priority}/10</span>
            </label>
            <div className="range-slider-container">
              <input
                type="range"
                min="1"
                max="10"
                value={formData.params.priority}
                onChange={e => onChange({
                  ...formData,
                  params: { ...formData.params, priority: parseInt(e.target.value) }
                })}
                className="range-slider"
              />
              <div className="range-labels">
                <span>Низкий</span>
                <span>Средний</span>
                <span>Высокий</span>
              </div>
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="form-field">
              <label className="required-field">Компания-заказчик</label>
              <input
                type="text"
                className="input"
                placeholder="Введите название компании"
                value={formData.params.customerCompanyName}
                onChange={e => onChange({
                  ...formData,
                  params: { ...formData.params, customerCompanyName: e.target.value }
                })}
                required
              />
            </div>
            
            <div className="form-field">
              <label className="required-field">Компания-исполнитель</label>
              <input
                type="text"
                className="input"
                placeholder="Введите название компании"
                value={formData.params.executorCompanyName}
                onChange={e => onChange({
                  ...formData,
                  params: { ...formData.params, executorCompanyName: e.target.value }
                })}
                required
              />
            </div>
          </div>
        </div>
      </div>

      <div className="edit-form-section">
        <h2 className="h2">Менеджер проекта</h2>
        
        <div className="space-y-6">
          <div className="employee-search-container">
            <label className="required-field">Поиск менеджера *</label>
            <div className="search-input-wrapper">
              <input
                type="text"
                className="input"
                placeholder="Поиск по имени, фамилии и..."
                value={managerQuery}
                onChange={e => handleSearchManager(e.target.value)}
              />
              {managerLoading && (
                <div className="search-loading"></div>
              )}
            </div>
          </div>

          {selectedManager && (
            <div className="selected-manager-card">
              <div className="selected-manager-info">
                <div>
                  <div className="manager-name">
                    {formatEmployeeName(selectedManager)}
                  </div>
                  <div className="manager-email">
                    {selectedManager.email}
                  </div>
                </div>
                <span className="badge badge-primary">Менеджер</span>
              </div>
            </div>
          )}

          {managerResults.length > 0 && (
            <div className="manager-selection-list">
              <div className="manager-selection-header">
                <span className="text-sm font-medium">Выберите менеджера:</span>
                <span className="text-xs text-gray-500">
                  {managerResults.length} сотрудников
                </span>
              </div>
              
              <div className="manager-grid">
                {managerResults.slice(0, 8).map(employee => (
                  <button
                    key={employee.id}
                    type="button"
                    className={`manager-option ${formData.managerId === employee.id ? 'selected' : ''}`}
                    onClick={() => handleSelectManager(employee)}
                    disabled={managerLoading}
                  >
                    <div className="manager-option-content">
                      <div className="manager-option-name">
                        {formatEmployeeName(employee)}
                      </div>
                      <div className="manager-option-email">
                        {employee.email}
                      </div>
                    </div>
                  </button>
                ))}
              </div>
            </div>
          )}

          {managerQuery && managerResults.length === 0 && !managerLoading && (
            <div className="empty-state">
              Сотрудники не найдены
            </div>
          )}
        </div>
      </div>

      <div className="edit-form-section">
        <div className="team-header">
          <h2 className="h2">Команда проекта</h2>
          <div className="flex items-center gap-2">
            <span className="selected-count">
              Выбрано: <span className="font-bold">{formData.employeesIds.length}</span>
            </span>
            {formData.employeesIds.length > 0 && (
              <button
                type="button"
                className="btn btn-sm btnDanger clear-team-btn"
                onClick={clearTeam}
              >
                Очистить
              </button>
            )}
          </div>
        </div>

        <div className="space-y-6">
          <div className="employees-search-container">
            <label>Поиск исполнителей</label>
            <input
              type="text"
              className="input"
              placeholder="Поиск сотрудников..."
              value={teamQuery}
              onChange={e => setTeamQuery(e.target.value)}
            />
          </div>

          {teamFiltered.length > 0 && (
            <div className="team-members-grid">
              {teamFiltered.map(employee => {
                const isSelected = formData.employeesIds.includes(employee.id);
                
                return (
                  <div
                    key={employee.id}
                    className="team-member-card"
                  >
                    <div className="team-member-info">
                      <div className="team-member-name">
                        {formatEmployeeName(employee)}
                      </div>
                      <div className="team-member-email">
                        {employee.email}
                      </div>
                    </div>
                    
                    <div className="team-member-actions">
                      <button
                        type="button"
                        className={`btn btn-sm ${isSelected ? 'btnDanger' : 'btnPrimary'}`}
                        onClick={() => handleToggleEmployee(employee.id)}
                      >
                        {isSelected ? "Убрать" : "Добавить"}
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>
          )}

          {teamLoading && (
            <div className="loading-state">
              <div className="loading-spinner"></div>
              <div className="text-sm text-gray-500">Загрузка...</div>
            </div>
          )}

          {!teamLoading && teamQuery && teamFiltered.length === 0 && (
            <div className="empty-state">
              Сотрудники не найдены
            </div>
          )}

          {!teamLoading && !teamQuery && teamFiltered.length === 0 && allEmployees.length > 0 && (
            <div className="empty-state">
              Все сотрудники уже в команде или являются менеджером
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ProjectEditForm;