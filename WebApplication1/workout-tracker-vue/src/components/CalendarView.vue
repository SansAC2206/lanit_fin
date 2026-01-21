<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Activity Calendar</h2>
      <div class="d-flex align-items-center">
        <button class="btn btn-outline-secondary me-2" @click="prevMonth">
          <i class="bi bi-chevron-left"></i>
        </button>
        <h4 class="mb-0">{{ monthYear }}</h4>
        <button class="btn btn-outline-secondary ms-2" @click="nextMonth">
          <i class="bi bi-chevron-right"></i>
        </button>
      </div>
    </div>
    
    <!-- Календарь -->
    <div class="calendar-grid mb-4">
      <div class="calendar-header">
        <div v-for="day in ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']" :key="day" class="calendar-header-day">
          {{ day }}
        </div>
      </div>
      
      <div class="calendar-body">
        <div v-for="day in calendarDays" :key="day.date" class="calendar-day" :class="{ 'not-current-month': !day.isCurrentMonth }">
          <div class="day-header">
            <span class="day-number">{{ day.dayNumber }}</span>
          </div>
          
          <div class="day-content">
            <div v-if="day.totalMinutes > 0" class="text-center">
              <div class="activity-indicator" :class="getActivityClass(day.totalMinutes)"></div>
              <small class="d-block mt-1">{{ day.totalMinutes }} min</small>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Детали выбранного дня -->
    <div v-if="selectedDay && selectedDay.activities.length > 0" class="card">
      <div class="card-header">
        <h5 class="mb-0">Activities for {{ formatDate(selectedDay.date) }}</h5>
      </div>
      <div class="card-body">
        <div class="mb-3">
          <div class="d-flex align-items-center">
            <div class="activity-indicator me-3" :class="getActivityClass(selectedDay.totalMinutes)" style="width: 40px; height: 40px;"></div>
            <div>
              <h4 class="mb-0">{{ selectedDay.totalMinutes }} minutes</h4>
              <p class="text-muted mb-0">Total for the day</p>
            </div>
          </div>
        </div>
        
        <div class="list-group">
          <div v-for="activity in selectedDay.activities" :key="activity.id" class="list-group-item">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h6 class="mb-1">{{ activity.exercise?.name }}</h6>
                <p class="mb-1 text-muted small">{{ activity.notes || 'No notes' }}</p>
              </div>
              <span class="badge bg-primary">{{ activity.minutes }} min</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import api from '@/api/client';

export default {
  name: 'CalendarView',
  data() {
    return {
      currentDate: new Date(),
      calendarDays: [],
      selectedDay: null,
      monthlyActivities: [],
    };
  },
  computed: {
    monthYear() {
      return this.currentDate.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
    },
  },
  async mounted() {
    await this.loadCalendarData();
  },
  methods: {
    async loadCalendarData() {
      const year = this.currentDate.getFullYear();
      const month = this.currentDate.getMonth() + 1;
      
      try {
        // Загружаем активности за месяц
        this.monthlyActivities = await api.getActivities({
          month: month,
          year: year
        });
        this.generateCalendar();
      } catch (error) {
        console.error('Error loading calendar data:', error);
      }
    },
    
    generateCalendar() {
      const year = this.currentDate.getFullYear();
      const month = this.currentDate.getMonth();
      const firstDay = new Date(year, month, 1);
      const lastDay = new Date(year, month + 1, 0);
      
      const calendarDays = [];
      
      // Находим первый день недели (понедельник)
      const firstDayOfWeek = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;
      
      // Добавляем дни предыдущего месяца
      const prevMonthLastDay = new Date(year, month, 0).getDate();
      for (let i = firstDayOfWeek - 1; i >= 0; i--) {
        const date = new Date(year, month - 1, prevMonthLastDay - i);
        calendarDays.push(this.createDayObject(date, false));
      }
      
      // Добавляем дни текущего месяца
      for (let i = 1; i <= lastDay.getDate(); i++) {
        const date = new Date(year, month, i);
        calendarDays.push(this.createDayObject(date, true));
      }
      
      // Добавляем дни следующего месяца
      const daysToAdd = 42 - calendarDays.length; // 6 недель
      for (let i = 1; i <= daysToAdd; i++) {
        const date = new Date(year, month + 1, i);
        calendarDays.push(this.createDayObject(date, false));
      }
      
      this.calendarDays = calendarDays;
    },
    
    createDayObject(date, isCurrentMonth) {
      const dateStr = date.toISOString().split('T')[0];
      
      // Находим активности для этого дня
      const dayActivities = this.monthlyActivities.filter(
        activity => new Date(activity.date).toISOString().split('T')[0] === dateStr
      );
      
      const totalMinutes = dayActivities.reduce((sum, activity) => sum + activity.minutes, 0);
      
      return {
        date: dateStr,
        dayNumber: date.getDate(),
        isCurrentMonth,
        totalMinutes,
        activities: dayActivities,
      };
    },
    
    getActivityClass(minutes) {
      if (minutes < 30) return 'low';
      if (minutes <= 90) return 'normal';
      return 'high';
    },
    
    async prevMonth() {
      this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() - 1, 1);
      await this.loadCalendarData();
      this.selectedDay = null;
    },
    
    async nextMonth() {
      this.currentDate = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 1);
      await this.loadCalendarData();
      this.selectedDay = null;
    },
    
    formatDate(dateString) {
      return new Date(dateString).toLocaleDateString();
    },
    
    selectDay(day) {
      this.selectedDay = { ...day };
    },
  },
};
</script>

<style scoped>
/* Стили остаются такими же как в предыдущей версии */
.calendar-grid {
  background: white;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  overflow: hidden;
}

.calendar-header {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  background-color: #f8f9fa;
  border-bottom: 2px solid #dee2e6;
}

.calendar-header-day {
  padding: 1rem;
  text-align: center;
  font-weight: 600;
  color: #495057;
}

.calendar-body {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 1px;
  background-color: #dee2e6;
}

.calendar-day {
  background: white;
  min-height: 100px;
  padding: 0.5rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.calendar-day:hover {
  background-color: #f8f9fa;
  transform: scale(1.02);
  z-index: 1;
}

.calendar-day.not-current-month {
  background-color: #f8f9fa;
  color: #adb5bd;
}

.day-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.day-number {
  font-weight: 600;
}

.day-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: calc(100% - 2rem);
}

.activity-indicator {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: inline-block;
}

.activity-indicator.low {
  background-color: #ffc107;
}

.activity-indicator.normal {
  background-color: #198754;
}

.activity-indicator.high {
  background-color: #dc3545;
}
</style>