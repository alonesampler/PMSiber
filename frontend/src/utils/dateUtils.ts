/**
 * Преобразует дату в формат, который понимает ASP.NET Core
 * Формат: "YYYY-MM-DDTHH:mm:ss"
 */
export const formatDateForApi = (dateString: string): string => {
  const date = new Date(dateString);
  
  // Получаем компоненты даты
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  const seconds = String(date.getSeconds()).padStart(2, '0');
  
  // Формат для ASP.NET: "2024-01-20T00:00:00"
  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
};

/**
 * Преобразует дату из API в формат для input[type="date"]
 */
export const formatDateForInput = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toISOString().split('T')[0];
};

/**
 * Проверяет, является ли строка валидной датой
 */
export const isValidDate = (dateString: string): boolean => {
  const date = new Date(dateString);
  return !isNaN(date.getTime());
};