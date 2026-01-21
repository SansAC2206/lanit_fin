<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Training Programs</h2>
      <button class="btn btn-success" @click="showAddForm">
        <i class="bi bi-plus-circle"></i> Add Program
      </button>
    </div>
    
    <!-- Форма программы -->
    <div v-if="showForm" class="card mb-4">
      <div class="card-body">
        <h5 class="card-title">
          <i class="bi" :class="editingProgram ? 'bi-pencil-square' : 'bi-plus-circle'"></i>
          {{ editingProgram ? 'Edit Program' : 'New Program' }}
        </h5>
        <form @submit.prevent="saveProgram">
          <div class="mb-3">
            <label class="form-label">Name *</label>
            <input v-model="form.name" type="text" class="form-control" required>
          </div>
          <div class="mb-3">
            <label class="form-label">Type *</label>
            <select v-model="form.type" class="form-select" required>
              <option value="">Select type...</option>
              <option value="Силовая">Strength</option>
              <option value="Кардио">Cardio</option>
              <option value="Растяжка">Stretching</option>
              <option value="Йога">Yoga</option>
              <option value="Пилатес">Pilates</option>
              <option value="Функциональная">Functional</option>
              <option value="Кроссфит">Crossfit</option>
            </select>
          </div>
          <div class="form-check mb-3">
            <input v-model="form.isActive" class="form-check-input" type="checkbox">
            <label class="form-check-label">Active</label>
          </div>
          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-primary" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ editingProgram ? 'Update' : 'Create' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="cancelForm" :disabled="loading">
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- Список программ -->
    <div class="table-responsive">
      <table class="table table-hover">
        <thead>
          <tr>
            <th>Name</th>
            <th>Type</th>
            <th>Status</th>
            <th>Created</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="program in programs" :key="program.id">
            <td>{{ program.name }}</td>
            <td><span class="badge bg-info">{{ program.type }}</span></td>
            <td>
              <span :class="['badge', program.isActive ? 'bg-success' : 'bg-secondary']">
                {{ program.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td>{{ formatDate(program.createdDate) }}</td>
            <td>
              <button class="btn btn-sm btn-outline-primary me-1" @click="editProgram(program)">
                <i class="bi bi-pencil"></i>
              </button>
              <button class="btn btn-sm btn-outline-danger" @click="deleteProgram(program.id)">
                <i class="bi bi-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    
    <div v-if="programs.length === 0" class="text-center py-5">
      <p class="text-muted">No programs found</p>
    </div>
  </div>
</template>

<script>
import api from '@/api/client';

export default {
  name: 'ProgramList',
  data() {
    return {
      programs: [],
      showForm: false,
      editingProgram: null,
      form: {
        name: '',
        type: '',
        isActive: true,
      },
      loading: false,
    };
  },
  mounted() {
    this.loadPrograms();
  },
  methods: {
    async loadPrograms() {
      try {
        const response = await api.getPrograms();
        this.programs = response.map(program => ({
          ...program,
          createdDate: program.createdDate || new Date().toISOString()
        }));
      } catch (error) {
        console.error('Error loading programs:', error);
        this.showError('Failed to load programs');
      }
    },
    
    showAddForm() {
      this.editingProgram = null;
      this.form = {
        name: '',
        type: '',
        isActive: true,
      };
      this.showForm = true;
    },
    
    editProgram(program) {
      this.editingProgram = program;
      this.form = {
        name: program.name,
        type: program.type,
        isActive: program.isActive,
      };
      this.showForm = true;
    },
    
    async saveProgram() {
      if (!this.form.name.trim() || !this.form.type.trim()) {
        this.showError('Please fill all required fields');
        return;
      }
      
      this.loading = true;
      try {
        const programData = {
          name: this.form.name.trim(),
          type: this.form.type,
          isActive: this.form.isActive,
        };
        
        console.log('Saving program data:', programData);
        console.log('Editing program:', this.editingProgram);
        
        if (this.editingProgram) {
          // ВАЖНО: Добавляем ID для обновления
          programData.id = this.editingProgram.id;
          await api.updateProgram(this.editingProgram.id, programData);
          this.showSuccess('Program updated successfully');
        } else {
          await api.createProgram(programData);
          this.showSuccess('Program created successfully');
        }
        
        await this.loadPrograms();
        this.cancelForm();
      } catch (error) {
        console.error('Error saving program:', error);
        this.showError(error.message || 'Error saving program');
      } finally {
        this.loading = false;
      }
    },
    
    cancelForm() {
      this.showForm = false;
      this.editingProgram = null;
      this.form = { name: '', type: '', isActive: true };
    },
    
    async deleteProgram(id) {
      if (!confirm('Are you sure you want to delete this program?')) return;
      
      try {
        await api.deleteProgram(id);
        await this.loadPrograms();
        this.showSuccess('Program deleted successfully');
      } catch (error) {
        console.error('Error deleting program:', error);
        this.showError(error.message || 'Error deleting program');
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