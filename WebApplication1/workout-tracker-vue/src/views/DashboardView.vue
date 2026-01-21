<template>
  <div>
    <h1 class="mb-4">Dashboard</h1>
    
    <!-- Статистика -->
    <div class="row mb-4">
      <div class="col-md-3 mb-3">
        <div class="card bg-primary text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h6 class="card-title">Total Activities</h6>
                <h2>{{ totalActivities }}</h2>
              </div>
              <i class="bi bi-activity display-4 opacity-50"></i>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-md-3 mb-3">
        <div class="card bg-success text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h6 class="card-title">Active Programs</h6>
                <h2>{{ activePrograms }}</h2>
              </div>
              <i class="bi bi-check-circle display-4 opacity-50"></i>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-md-3 mb-3">
        <div class="card bg-info text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h6 class="card-title">Active Exercises</h6>
                <h2>{{ activeExercisesCount }}</h2>
              </div>
              <i class="bi bi-bicycle display-4 opacity-50"></i>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-md-3 mb-3">
        <div class="card bg-warning text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h6 class="card-title">Today's Minutes</h6>
                <h2>{{ todayMinutes }}</h2>
                <small v-if="todayActivityCount > 0">{{ todayActivityCount }} activities</small>
                <small v-else>No activities today</small>
              </div>
              <i class="bi bi-clock display-4 opacity-50"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Прогресс сегодняшнего дня -->
    <div class="row mb-4">
      <div class="col-md-12">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">
              <i class="bi bi-calendar-day me-2"></i>Today's Activity Progress
            </h5>
          </div>
          <div class="card-body">
            <div class="row align-items-center">
              <div class="col-md-3">
                <div class="text-center">
                  <h3 class="text-primary">{{ todayMinutes }} / 1440</h3>
                  <p class="text-muted">Minutes today</p>
                </div>
              </div>
              <div class="col-md-9">
                <div class="progress" style="height: 25px;">
                  <div 
                    class="progress-bar" 
                    :class="progressBarClass"
                    :style="{ width: todayProgress + '%' }"
                    role="progressbar"
                  >
                    {{ Math.round(todayProgress) }}%
                  </div>
                </div>
                <div class="d-flex justify-content-between mt-2">
                  <small class="text-muted">Daily limit: 1440 minutes</small>
                  <small :class="limitWarningClass">{{ limitWarningText }}</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Быстрые действия -->
    <div class="row mb-4">
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Quick Actions</h5>
          </div>
          <div class="card-body">
            <div class="d-grid gap-2">
              <router-link to="/activities" class="btn btn-primary">
                <i class="bi bi-plus-circle me-2"></i>Add Today's Activity
              </router-link>
              <router-link to="/exercises" class="btn btn-success">
                <i class="bi bi-plus-circle me-2"></i>Add New Exercise
              </router-link>
              <router-link to="/programs" class="btn btn-info">
                <i class="bi bi-plus-circle me-2"></i>Add New Program
              </router-link>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Recent Activities</h5>
          </div>
          <div class="card-body">
            <div v-if="recentActivities.length > 0">
              <div v-for="activity in recentActivities" :key="activity.id" class="mb-2 p-2 border rounded">
                <div class="d-flex justify-content-between">
                  <div>
                    <strong>{{ getExerciseName(activity.exerciseId) }}</strong>
                    <small class="d-block text-muted">{{ formatDate(activity.date) }}</small>
                    <small class="text-muted" v-if="activity.notes">{{ activity.notes }}</small>
                  </div>
                  <span class="badge bg-primary align-self-start">{{ activity.minutes }} min</span>
                </div>
              </div>
            </div>
            <div v-else class="text-center py-4">
              <i class="bi bi-calendar-x display-5 text-muted mb-3"></i>
              <p class="text-muted">No recent activities</p>
              <router-link to="/activities" class="btn btn-sm btn-outline-primary">
                Add your first activity
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Статистика по программам -->
    <div class="row" v-if="programStats.length > 0">
      <div class="col-md-12">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Program Statistics</h5>
          </div>
          <div class="card-body">
            <div class="row">
              <div v-for="program in programStats" :key="program.id" class="col-md-4 mb-3">
                <div class="card h-100">
                  <div class="card-body">
                    <h6 class="card-title">{{ program.name }}</h6>
                    <p class="card-text">
                      <span class="badge bg-info me-2">{{ program.type }}</span>
                      <span :class="['badge', program.isActive ? 'bg-success' : 'bg-secondary']">
                        {{ program.isActive ? 'Active' : 'Inactive' }}
                      </span>
                    </p>
                    <div class="mt-2">
                      <small class="text-muted">Exercises: {{ program.exerciseCount }}</small>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue';
import api from '@/api/client';

export default {
  name: 'Dashboard',
  setup() {
    const totalActivities = ref(0);
    const activePrograms = ref(0);
    const activeExercisesCount = ref(0);
    const todayMinutes = ref(0);
    const todayActivityCount = ref(0);
    const recentActivities = ref([]);
    const allExercises = ref([]);
    const programs = ref([]);
    const programStats = ref([]);

    // Вычисляемые свойства
    const todayProgress = computed(() => {
      return (todayMinutes.value / 1440) * 100;
    });

    const progressBarClass = computed(() => {
      if (todayProgress.value < 30) return 'bg-success';
      if (todayProgress.value < 70) return 'bg-warning';
      return 'bg-danger';
    });

    const limitWarningClass = computed(() => {
      if (todayMinutes.value === 0) return 'text-muted';
      if (todayMinutes.value < 30) return 'text-warning';
      if (todayMinutes.value < 90) return 'text-success';
      if (todayMinutes.value < 1440) return 'text-danger';
      return 'text-danger fw-bold';
    });

    const limitWarningText = computed(() => {
      if (todayMinutes.value === 0) return 'No activities today';
      if (todayMinutes.value < 30) return 'Low activity';
      if (todayMinutes.value < 90) return 'Normal activity';
      if (todayMinutes.value < 1440) return 'High activity';
      return 'Daily limit reached!';
    });

    const loadDashboardData = async () => {
      try {
        // Загружаем все данные параллельно
        const [
          programsData,
          exercisesData,
          activitiesData,
          todaySummaryData
        ] = await Promise.all([
          api.getPrograms(),
          api.getExercises(),
          api.getActivities(),
          api.getTodaySummary()
        ]);

        // Сохраняем данные
        programs.value = programsData;
        allExercises.value = exercisesData;

        // Рассчитываем статистику
        totalActivities.value = activitiesData.length;
        activePrograms.value = programsData.filter(p => p.isActive).length;
        activeExercisesCount.value = exercisesData.filter(e => e.isActive).length;

        // Данные за сегодня
        todayMinutes.value = todaySummaryData.totalMinutes || 0;
        todayActivityCount.value = todaySummaryData.activityCount || 0;

        // Последние 5 активностей
        recentActivities.value = activitiesData
          .sort((a, b) => new Date(b.date) - new Date(a.date))
          .slice(0, 5);

        // Статистика по программам
        programStats.value = programsData.map(program => {
          const programExercises = exercisesData.filter(e => 
            e.trainingProgramId === program.id
          );
          return {
            id: program.id,
            name: program.name,
            type: program.type,
            isActive: program.isActive,
            exerciseCount: programExercises.length
          };
        }).filter(p => p.exerciseCount > 0).slice(0, 6); // Показываем максимум 6 программ

      } catch (error) {
        console.error('Error loading dashboard data:', error);
        // Если API для сегодняшнего дня недоступен, рассчитываем сами
        await loadFallbackData();
      }
    };

    const loadFallbackData = async () => {
      try {
        const [programsData, exercisesData, activitiesData] = await Promise.all([
          api.getPrograms(),
          api.getExercises(),
          api.getActivities()
        ]);

        programs.value = programsData;
        allExercises.value = exercisesData;

        // Базовая статистика
        totalActivities.value = activitiesData.length;
        activePrograms.value = programsData.filter(p => p.isActive).length;
        activeExercisesCount.value = exercisesData.filter(e => e.isActive).length;

        // Рассчитываем данные за сегодня вручную
        const today = new Date().toISOString().split('T')[0];
        const todayActivities = activitiesData.filter(a => 
          new Date(a.date).toISOString().split('T')[0] === today
        );
        
        todayMinutes.value = todayActivities.reduce((sum, a) => sum + a.minutes, 0);
        todayActivityCount.value = todayActivities.length;

        // Последние активности
        recentActivities.value = activitiesData
          .sort((a, b) => new Date(b.date) - new Date(a.date))
          .slice(0, 5);

      } catch (error) {
        console.error('Error loading fallback data:', error);
      }
    };

    const formatDate = (dateString) => {
      try {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return 'N/A';
        
        // Если это сегодня
        const today = new Date();
        if (date.toDateString() === today.toDateString()) {
          return 'Today';
        }
        
        // Если это вчера
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);
        if (date.toDateString() === yesterday.toDateString()) {
          return 'Yesterday';
        }
        
        return date.toLocaleDateString('en-US', {
          month: 'short',
          day: 'numeric'
        });
      } catch (error) {
        return 'N/A';
      }
    };

    const getExerciseName = (exerciseId) => {
      const exercise = allExercises.value.find(e => e.id === exerciseId);
      return exercise ? exercise.name : 'Unknown Exercise';
    };

    const refreshData = () => {
      loadDashboardData();
    };

    onMounted(() => {
      loadDashboardData();
    });

    return {
      totalActivities,
      activePrograms,
      activeExercisesCount,
      todayMinutes,
      todayActivityCount,
      recentActivities,
      programStats,
      todayProgress,
      progressBarClass,
      limitWarningClass,
      limitWarningText,
      formatDate,
      getExerciseName,
      refreshData
    };
  },
};
</script>

<style scoped>
.card {
  transition: transform 0.2s;
  border: none;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.progress {
  border-radius: 10px;
  overflow: hidden;
}

.progress-bar {
  transition: width 0.5s ease;
}

.badge {
  font-size: 0.8em;
}

.text-primary {
  color: #0d6efd !important;
}

.display-4 {
  font-size: 2.5rem;
  opacity: 0.7;
}

.card-title {
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 0.5rem;
}

.card h2 {
  margin-bottom: 0.25rem;
  font-weight: 600;
}
</style>