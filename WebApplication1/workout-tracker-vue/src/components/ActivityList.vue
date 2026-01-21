<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Workout Activities</h2>
      <button class="btn btn-success" @click="showForm = true">
        <i class="bi bi-plus-circle"></i> Add Activity
      </button>
    </div>
    
    <!-- Фильтры -->
    <div class="row mb-4">
      <div class="col-md-4">
        <label class="form-label">Date</label>
        <input v-model="filters.date" type="date" class="form-control" @change="loadActivities">
      </div>
      <div class="col-md-4">
        <label class="form-label">Exercise</label>
        <select v-model="filters.exerciseId" class="form-select" @change="loadActivities">
          <option value="">All exercises</option>
          <option v-for="exercise in activeExercises" :key="exercise.id" :value="exercise.id">
            {{ exercise.name }}
          </option>
        </select>
      </div>
    </div>
    
    <!-- Форма активности -->
    <div v-if="showForm" class="card mb-4">
      <div class="card-body">
        <h5 class="card-title">{{ editingActivity ? 'Edit Activity' : 'New Activity' }}</h5>
        <form @submit.prevent="saveActivity">
          <div class="row mb-3">
            <div class="col-md-6">
              <label class="form-label">Date *</label>
              <input v-model="form.date" type="date" class="form-control" required>
            </div>
            <div class="col-md-6">
              <label class="form-label">Minutes *</label>
              <input v-model="form.minutes" type="number" class="form-control" min="1" max="1440" required>
            </div>
          </div>
          <div class="mb-3">
            <label class="form-label">Exercise *</label>
            <select v-model="form.exerciseId" class="form-select" required :disabled="isExerciseDisabled">
              <option value="">Select exercise...</option>
              <option v-for="exercise in activeExercises" :key="exercise.id" :value="exercise.id">
                {{ exercise.name }} ({{ exercise.trainingProgram?.name }})
              </option>
            </select>
          </div>
          <div class="mb-3">
            <label class="form-label">Notes</label>
            <textarea v-model="form.notes" class="form-control" rows="3"></textarea>
          </div>
          <div v-if="dailyLimitWarning" class="alert alert-warning mb-3">
            {{ dailyLimitWarning }}
          </div>
          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-primary" :disabled="loading">
              {{ loading ? 'Saving...' : 'Save' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="cancelForm">
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- Статистика дня -->
    <div v-if="dailySummary" class="card mb-4">
      <div class="card-body">
        <div class="row align-items-center">
          <div class="col-md-4">
            <h5 class="mb-0">{{ formatDate(dailySummary.date) }}</h5>
            <p class="text-muted mb-0">
              Total: {{ dailySummary.totalMinutes }} minutes | 
              Activities: {{ dailySummary.activityCount }}
            </p>
          </div>
          <div class="col-md-8">
            <div class="progress" style="height: 20px;">
              <div 
                class="progress-bar" 
                :class="progressBarClass"
                :style="{ width: dailyProgress + '%' }"
              >
                {{ dailySummary.totalMinutes }} / 1440 minutes
              </div>
            </div>
            <div class="mt-2">
              <span class="badge" :class="stickerColorClass">
                Activity level: {{ dailySummary.stickerColor }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Список активностей -->
    <div class="list-group">
      <div v-for="activity in activities" :key="activity.id" class="list-group-item">
        <div class="d-flex justify-content-between align-items-center">
          <div>
            <h6 class="mb-1">{{ activity.exercise?.name }}</h6>
            <p class="mb-1 text-muted small">{{ activity.notes || 'No notes' }}</p>
            <small class="text-muted">
              Program: {{ activity.exercise?.trainingProgram?.name }}
            </small>
          </div>
          <div class="text-end">
            <div class="mb-2">
              <span class="badge bg-primary fs-6">{{ activity.minutes }} min</span>
            </div>
            <div>
              <small class="text-muted d-block">{{ formatDate(activity.date) }}</small>
              <div class="mt-2">
                <button class="btn btn-sm btn-outline-primary me-1" @click="editActivity(activity)">
                  <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" @click="deleteActivity(activity.id)">
                  <i class="bi bi-trash"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <div v-if="activities.length === 0" class="text-center py-5">
      <p class="text-muted">No activities found</p>
    </div>
  </div>
</template>

<script>
import api from '@/api/client';

export default {
  name: 'ActivityList',
  data() {
    return {
      activities: [],
      activeExercises: [],
      dailySummary: null,
      showForm: false,
      editingActivity: null,
      filters: {
        date: new Date().toISOString().split('T')[0],
        exerciseId: '',
      },
      form: {
        date: new Date().toISOString().split('T')[0],
        minutes: 30,
        exerciseId: '',
        notes: '',
      },
      loading: false,
      isExerciseDisabled: false,
    };
  },
  computed: {
    dailyProgress() {
      if (!this.dailySummary) return 0;
      return Math.min((this.dailySummary.totalMinutes / 1440) * 100, 100);
    },
    progressBarClass() {
      if (this.dailyProgress < 30) return 'bg-success';
      if (this.dailyProgress < 70) return 'bg-warning';
      return 'bg-danger';
    },
    stickerColorClass() {
      if (!this.dailySummary) return 'bg-secondary';
      switch(this.dailySummary.stickerColor) {
        case 'yellow': return 'bg-warning';
        case 'green': return 'bg-success';
        case 'red': return 'bg-danger';
        default: return 'bg-secondary';
      }
    },
    dailyLimitWarning() {
      if (!this.dailySummary || !this.form.minutes) return null;
      const total = this.dailySummary.totalMinutes + parseInt(this.form.minutes);
      if (total > 1440) {
        return `Daily limit exceeded! Available: ${1440 - this.dailySummary.totalMinutes} minutes`;
      }
      return null;
    },
  },
  async mounted() {
    await this.loadActiveExercises();
    await this.loadActivities();
  },
  methods: {
    async loadActivities() {
      try {
        // Загружаем активности за выбранный день
        const activities = await api.getActivities({
          date: this.filters.date
        });
        
        // Фильтруем по упражнению если выбран
        let filteredActivities = activities;
        if (this.filters.exerciseId) {
          filteredActivities = filteredActivities.filter(
            activity => activity.exerciseId === parseInt(this.filters.exerciseId)
          );
        }
        
        this.activities = filteredActivities;
        
        // Загружаем статистику дня
        await this.loadDailySummary();
      } catch (error) {
        console.error('Error loading activities:', error);
      }
    },
    
    async loadDailySummary() {
      try {
        this.dailySummary = await api.getDailySummary(this.filters.date);
      } catch (error) {
        console.error('Error loading daily summary:', error);
        // Если API не возвращает summary, рассчитываем сами
        const activities = this.activities.filter(a => 
          new Date(a.date).toISOString().split('T')[0] === this.filters.date
        );
        
        const totalMinutes = activities.reduce((sum, a) => sum + a.minutes, 0);
        const stickerColor = totalMinutes < 30 ? 'yellow' :
                           totalMinutes <= 90 ? 'green' : 'red';
        
        this.dailySummary = {
          date: this.filters.date,
          totalMinutes,
          stickerColor,
          activityCount: activities.length
        };
      }
    },
    
    async loadActiveExercises() {
      try {
        this.activeExercises = await api.getActiveExercises();
        if (this.activeExercises.length > 0 && !this.form.exerciseId) {
          this.form.exerciseId = this.activeExercises[0].id;
        }
      } catch (error) {
        console.error('Error loading exercises:', error);
      }
    },
    
    async saveActivity() {
      if (!this.validateActivity()) return;
      
      this.loading = true;
      try {
        const activityData = {
          date: this.form.date,
          minutes: parseInt(this.form.minutes),
          exerciseId: parseInt(this.form.exerciseId),
          notes: this.form.notes?.trim() || '',
        };
        
        if (this.editingActivity) {
          await api.updateActivity(this.editingActivity.id, activityData);
        } else {
          await api.createActivity(activityData);
        }
        
        await this.loadActivities();
        this.cancelForm();
      } catch (error) {
        console.error('Error saving activity:', error);
        alert(error.response?.data || 'Error saving activity');
      } finally {
        this.loading = false;
      }
    },
    
    editActivity(activity) {
      this.editingActivity = activity;
      this.form = {
        date: new Date(activity.date).toISOString().split('T')[0],
        minutes: activity.minutes,
        exerciseId: activity.exerciseId,
        notes: activity.notes || '',
      };
      
      // Check if exercise is still active
      const isExerciseActive = this.activeExercises.some(e => e.id === activity.exerciseId);
      this.isExerciseDisabled = !isExerciseActive;
      
      this.showForm = true;
    },
    
    cancelForm() {
      this.showForm = false;
      this.editingActivity = null;
      this.isExerciseDisabled = false;
      this.form = {
        date: new Date().toISOString().split('T')[0],
        minutes: 30,
        exerciseId: this.activeExercises[0]?.id || '',
        notes: '',
      };
    },
    
    async deleteActivity(id) {
      if (!confirm('Are you sure you want to delete this activity?')) return;
      
      try {
        await api.deleteActivity(id);
        await this.loadActivities();
      } catch (error) {
        console.error('Error deleting activity:', error);
      }
    },
    
    validateActivity() {
      if (!this.form.date) {
        alert('Please select a date');
        return false;
      }
      
      if (!this.form.minutes || this.form.minutes < 1 || this.form.minutes > 1440) {
        alert('Minutes must be between 1 and 1440');
        return false;
      }
      
      if (!this.form.exerciseId) {
        alert('Please select an exercise');
        return false;
      }
      
      return true;
    },
    
    formatDate(dateString) {
      return new Date(dateString).toLocaleDateString();
    },
  },
};
</script>