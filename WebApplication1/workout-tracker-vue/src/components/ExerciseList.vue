<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Exercises</h2>
      <button class="btn btn-success" @click="showAddForm">
        <i class="bi bi-plus-circle"></i> Add Exercise
      </button>
    </div>
    
    <!-- Форма упражнения -->
    <div v-if="showForm" class="card mb-4">
      <div class="card-body">
        <h5 class="card-title">
          <i class="bi" :class="editingExercise ? 'bi-pencil-square' : 'bi-plus-circle'"></i>
          {{ editingExercise ? 'Edit Exercise' : 'New Exercise' }}
        </h5>
        <form @submit.prevent="saveExercise">
          <div class="mb-3">
            <label class="form-label">Name *</label>
            <input v-model="form.name" type="text" class="form-control" required>
          </div>
          <div class="mb-3">
            <label class="form-label">Program *</label>
            <select v-model="form.trainingProgramId" class="form-select" required>
              <option value="">Select program...</option>
              <option v-for="program in programs" :key="program.id" :value="program.id">
                {{ program.name }} ({{ program.type }})
              </option>
            </select>
          </div>
          <div class="form-check mb-3">
            <input v-model="form.isActive" class="form-check-input" type="checkbox">
            <label class="form-check-label">Active</label>
          </div>
          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-primary" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ editingExercise ? 'Update' : 'Create' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="cancelForm" :disabled="loading">
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- Список упражнений -->
    <div class="table-responsive">
      <table class="table table-hover">
        <thead>
          <tr>
            <th>Name</th>
            <th>Program</th>
            <th>Status</th>
            <th>Created</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="exercise in exercises" :key="exercise.id">
            <td>{{ exercise.name }}</td>
            <td>
              <span v-if="exercise.trainingProgramName" class="badge bg-info">
                {{ exercise.trainingProgramName }}
              </span>
              <span v-else class="text-muted">No program</span>
            </td>
            <td>
              <span :class="['badge', exercise.isActive ? 'bg-success' : 'bg-secondary']">
                {{ exercise.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td>{{ formatDate(exercise.createdDate) }}</td>
            <td>
              <button class="btn btn-sm btn-outline-primary me-1" @click="editExercise(exercise)">
                <i class="bi bi-pencil"></i>
              </button>
              <button class="btn btn-sm btn-outline-danger" @click="deleteExercise(exercise.id)">
                <i class="bi bi-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    
    <div v-if="exercises.length === 0" class="text-center py-5">
      <p class="text-muted">No exercises found</p>
    </div>
  </div>
</template>

<script>
import api from '@/api/client';

export default {
  name: 'ExerciseList',
  data() {
    return {
      exercises: [],
      programs: [],
      showForm: false,
      editingExercise: null,
      form: {
        name: '',
        trainingProgramId: '',
        isActive: true,
      },
      loading: false,
    };
  },
  async mounted() {
    await Promise.all([this.loadExercises(), this.loadPrograms()]);
  },
  methods: {
    async loadExercises() {
      try {
        const response = await api.getExercises();
        this.exercises = response.map(exercise => ({
          id: exercise.id,
          name: exercise.name,
          trainingProgramId: exercise.trainingProgramId,
          trainingProgramName: exercise.trainingProgramName || 
                              (exercise.trainingProgram?.name || 'No program'),
          isActive: exercise.isActive,
          createdDate: exercise.createdDate || new Date().toISOString()
        }));
      } catch (error) {
        console.error('Error loading exercises:', error);
        this.showError('Failed to load exercises');
      }
    },
    
    async loadPrograms() {
      try {
        this.programs = await api.getPrograms();
      } catch (error) {
        console.error('Error loading programs:', error);
        this.showError('Failed to load programs');
      }
    },
    
    showAddForm() {
      this.editingExercise = null;
      this.form = {
        name: '',
        trainingProgramId: this.programs.length > 0 ? this.programs[0].id.toString() : '',
        isActive: true,
      };
      this.showForm = true;
    },
    
    editExercise(exercise) {
      this.editingExercise = exercise;
      this.form = { 
        name: exercise.name,
        trainingProgramId: exercise.trainingProgramId.toString(),
        isActive: exercise.isActive,
      };
      this.showForm = true;
    },
    
    async saveExercise() {
      if (!this.form.name.trim() || !this.form.trainingProgramId) {
        this.showError('Please fill all required fields');
        return;
      }
      
      this.loading = true;
      try {
        const exerciseData = {
          name: this.form.name.trim(),
          trainingProgramId: parseInt(this.form.trainingProgramId),
          isActive: this.form.isActive,
        };
        
        console.log('Saving exercise data:', exerciseData);
        console.log('Editing exercise:', this.editingExercise);
        
        if (this.editingExercise) {
          // ВАЖНО: Добавляем ID для обновления
          exerciseData.id = this.editingExercise.id;
          await api.updateExercise(this.editingExercise.id, exerciseData);
          this.showSuccess('Exercise updated successfully');
        } else {
          await api.createExercise(exerciseData);
          this.showSuccess('Exercise created successfully');
        }
        
        await this.loadExercises();
        this.cancelForm();
      } catch (error) {
        console.error('Error saving exercise:', error);
        this.showError(error.message || 'Error saving exercise');
      } finally {
        this.loading = false;
      }
    },
    
    cancelForm() {
      this.showForm = false;
      this.editingExercise = null;
      this.form = { 
        name: '', 
        trainingProgramId: this.programs.length > 0 ? this.programs[0].id.toString() : '', 
        isActive: true 
      };
    },
    
    async deleteExercise(id) {
      if (!confirm('Are you sure you want to delete this exercise?')) return;
      
      try {
        await api.deleteExercise(id);
        await this.loadExercises();
        this.showSuccess('Exercise deleted successfully');
      } catch (error) {
        console.error('Error deleting exercise:', error);
        this.showError(error.message || 'Error deleting exercise');
      }
    },
    
    formatDate(dateString) {
      try {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return 'N/A';
        return date.toLocaleDateString('en-US', {
          year: 'numeric',
          month: 'short',
          day: 'numeric'
        });
      } catch (error) {
        return 'N/A';
      }
    },
    
    showError(message) {
      alert(`Error: ${message}`);
    },
    
    showSuccess(message) {
      alert(`Success: ${message}`);
    },
  },
};
</script>