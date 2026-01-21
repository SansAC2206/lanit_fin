<template>
    <div class="activity-calendar">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2><i class="bi bi-calendar3 me-2"></i>Календарь активностей</h2>
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

        <!-- ������� -->
        <div class="row mb-4">
            <div class="col">
                <div class="d-flex flex-wrap gap-3">
                    <div class="d-flex align-items-center">
                        <div class="activity-indicator low me-2"></div>
                        <small>Низкая активность (&lt; 30 мин)</small>
                    </div>
                    <div class="d-flex align-items-center">
                        <div class="activity-indicator normal me-2"></div>
                        <small>Нормальная активность (30-90 мин)</small>
                    </div>
                    <div class="d-flex align-items-center">
                        <div class="activity-indicator high me-2"></div>
                        <small>Высокая активность (&gt; 90 мин)</small>
                    </div>
                    <div class="d-flex align-items-center">
                        <div class="activity-indicator none me-2"></div>
                        <small>Нет активности</small>
                    </div>
                </div>
            </div>
        </div>

        <!-- ��������� -->
        <div class="calendar-grid mb-4">
            <div class="calendar-header">
                <div v-for="day in ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс']"
                     :key="day"
                     class="calendar-header-day">
                    {{ day }}
                </div>
            </div>

            <div class="calendar-body">
                <div v-for="day in calendarDays"
                     :key="day.date.toString()"
                     class="calendar-day"
                     :class="{
            'not-current-month': !day.isCurrentMonth,
            'today': isToday(day.date)
          }"
                     @click="selectDay(day)">
                    <div class="day-header">
                        <span class="day-number">{{ day.dayNumber }}</span>
                        <span v-if="isToday(day.date)" class="badge bg-primary ms-1">Сегодня</span>
                    </div>

                    <div class="day-content">
                        <div v-if="day.totalMinutes > 0" class="text-center">
                            <div class="activity-summary">
                                <ActivityIndicator :minutes="day.totalMinutes" size="sm" />
                                <small class="d-block mt-1">{{ day.totalMinutes }} мин</small>
                            </div>
                        </div>
                        <div v-else class="text-center text-muted">
                            <i class="bi bi-dash-lg"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- ������ ���������� ��� -->
        <div v-if="selectedDay" class="card">
            <div class="card-header">
                <h5 class="mb-0">
                    <i class="bi bi-calendar-event me-2"></i>
                    Активности за {{ formatDate(selectedDay.date) }}
                </h5>
            </div>
            <div class="card-body">
                <div class="row align-items-center mb-3">
                    <div class="col-md-6">
                        <div class="d-flex align-items-center">
                            <ActivityIndicator :minutes="selectedDay.totalMinutes" size="lg" />
                            <div class="ms-3">
                                <h4 class="mb-0">{{ selectedDay.totalMinutes }} минут</h4>
                                <p class="text-muted mb-0">Всего за день</p>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6 text-end">
                        <button class="btn btn-primary" @click="addActivityForSelectedDay">
                            <i class="bi bi-plus-circle me-1"></i>Добавить активность
                        </button>
                    </div>
                </div>

                <div v-if="selectedDay.activities.length > 0" class="mt-3">
                    <h6>Список активностей:</h6>
                    <div class="list-group">
                        <div v-for="activity in selectedDay.activities"
                             :key="activity.id"
                             class="list-group-item">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <h6 class="mb-1">{{ activity.exerciseName }}</h6>
                                    <p class="mb-1 text-muted">{{ activity.notes }}</p>
                                    <small class="text-muted">
                                        {{ activity.trainingProgramName }}
                                    </small>
                                </div>
                                <div class="text-end">
                                    <span class="badge bg-primary fs-6">{{ activity.minutes }} мин</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div v-else class="text-center py-4">
                    <i class="bi bi-emoji-frown display-5 text-muted mb-3"></i>
                    <p class="text-muted">На этот день нет запланированных активностей</p>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import {
  format,
  startOfMonth,
  endOfMonth,
  eachDayOfInterval,
  isSameDay,
  addMonths,
  subMonths,
  isToday as isTodayDate
} from 'date-fns'
import { ru } from 'date-fns/locale'
import workoutService from '@/services/workoutService'
import ActivityIndicator from './ActivityIndicator.vue'

const currentDate = ref(new Date())
const calendarDays = ref([])
const selectedDay = ref(null)

const monthYear = computed(() => {
  return format(currentDate.value, 'MMMM yyyy', { locale: ru })
})

onMounted(async () => {
  await generateCalendar()
})

async function generateCalendar() {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth() + 1

  try {
    calendarDays.value = await workoutService.generateCalendar(year, month)
  } catch (error) {
    console.error('Error generating calendar:', error)
  }
}

function isToday(date) {
  return isTodayDate(date)
}

function formatDate(date) {
  return format(date, 'dd MMMM yyyy', { locale: ru })
}

function selectDay(day) {
  selectedDay.value = { ...day }
}

async function prevMonth() {
  currentDate.value = subMonths(currentDate.value, 1)
  await generateCalendar()
  selectedDay.value = null
}

async function nextMonth() {
  currentDate.value = addMonths(currentDate.value, 1)
  await generateCalendar()
  selectedDay.value = null
}

function addActivityForSelectedDay() {
  if (selectedDay.value) {
    // ����� ����� ������� ����� ���������� ����������
    // ��� ������������� �� �������� �����������
    console.log('Add activity for:', selectedDay.value.date)
  }
}
</script>

<style scoped>
    .activity-calendar {
        padding: 1rem;
    }

    .calendar-grid {
        background: white;
        border-radius: 12px;
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
        min-height: 120px;
        padding: 0.75rem;
        cursor: pointer;
        transition: all 0.2s ease;
    }

        .calendar-day:hover {
            background-color: #f8f9fa;
            transform: scale(1.02);
            z-index: 1;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }

        .calendar-day.not-current-month {
            background-color: #f8f9fa;
            color: #adb5bd;
        }

        .calendar-day.today {
            background-color: #e7f1ff;
        }

    .day-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 0.5rem;
    }

    .day-number {
        font-weight: 600;
        font-size: 1.1rem;
    }

    .day-content {
        height: calc(100% - 2rem);
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .activity-summary {
        padding: 0.5rem;
        border-radius: 6px;
        background-color: rgba(0,0,0,0.03);
    }

    .activity-indicator {
        width: 20px;
        height: 20px;
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

        .activity-indicator.none {
            background-color: #dee2e6;
            border: 1px solid #adb5bd;
        }
</style>