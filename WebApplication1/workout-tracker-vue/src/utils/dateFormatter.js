export function formatDate(dateString) {
  try {
    if (!dateString) return 'N/A';
    
    let date;
    
    // Если это уже объект Date
    if (dateString instanceof Date) {
      date = dateString;
    } 
    // Если это строка
    else if (typeof dateString === 'string') {
      // Пробуем стандартный парсинг
      date = new Date(dateString);
      
      // Если невалидно, пробуем разные форматы
      if (isNaN(date.getTime())) {
        // Убираем возможные проблемы с форматом
        const cleanDateString = dateString.replace('Z', '').replace('T', ' ');
        date = new Date(cleanDateString);
        
        if (isNaN(date.getTime())) {
          // Пробуем разобрать как "YYYY-MM-DD"
          const isoMatch = dateString.match(/(\d{4})-(\d{2})-(\d{2})/);
          if (isoMatch) {
            date = new Date(`${isoMatch[1]}-${isoMatch[2]}-${isoMatch[3]}`);
          } else {
            // Пробуем как timestamp
            const timestamp = parseInt(dateString);
            if (!isNaN(timestamp)) {
              date = new Date(timestamp);
            }
          }
        }
      }
    }
    // Если это число (timestamp)
    else if (typeof dateString === 'number') {
      date = new Date(dateString);
    }
    
    if (!date || isNaN(date.getTime())) {
      console.warn('Invalid date:', dateString);
      return 'N/A';
    }
    
    // Форматируем дату
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    }).replace(',', '');
    
  } catch (error) {
    console.error('Error formatting date:', dateString, error);
    return 'Date error';
  }
}

export function formatDateOnly(dateString) {
  try {
    if (!dateString) return 'N/A';
    
    let date;
    
    if (dateString instanceof Date) {
      date = dateString;
    } else if (typeof dateString === 'string') {
      date = new Date(dateString);
      
      if (isNaN(date.getTime())) {
        // Попробуем разобрать дату без времени
        const match = dateString.match(/(\d{4})-(\d{2})-(\d{2})/);
        if (match) {
          date = new Date(parseInt(match[1]), parseInt(match[2]) - 1, parseInt(match[3]));
        }
      }
    }
    
    if (!date || isNaN(date.getTime())) {
      return 'N/A';
    }
    
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  } catch (error) {
    console.error('Error formatting date only:', dateString, error);
    return 'N/A';
  }
}