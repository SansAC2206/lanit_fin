import axios from 'axios';

const API_BASE_URL = 'http://localhost:5083/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

export default {
  // Training Programs
  getPrograms() {
    return apiClient.get('/programs');
  },
  
  getProgram(id) {
    return apiClient.get(`/programs/${id}`);
  },
  
  createProgram(program) {
    return apiClient.post('/programs', program);
  },
  
  updateProgram(id, program) {
    return apiClient.put(`/programs/${id}`, program);
  },
  
  deleteProgram(id) {
    return apiClient.delete(`/programs/${id}`);
  },
  
  // Exercises
  getExercises() {
    return apiClient.get('/exercises');
  },
  
  getActiveExercises() {
    return apiClient.get('/activities/active'); // Важно: этот метод в ActivitiesController!
  },
  
  getExercise(id) {
    return apiClient.get(`/exercises/${id}`);
  },
  
  createExercise(exercise) {
    return apiClient.post('/exercises', exercise);
  },
  
  updateExercise(id, exercise) {
    return apiClient.put(`/exercises/${id}`, exercise);
  },
  
  deleteExercise(id) {
    return apiClient.delete(`/exercises/${id}`);
  },
  
  // Workout Activities
  getActivities(params = {}) {
    return apiClient.get('/activities', { params });
  },
  
  getActivity(id) {
    return apiClient.get(`/activities/${id}`);
  },

  getTodaySummary() {
    return apiClient.get('/activities/today-summary');
  },
  
  createActivity(activity) {
    return apiClient.post('/activities', activity);
  },
  
  updateActivity(id, activity) {
    return apiClient.put(`/activities/${id}`, activity);
  },
  
  deleteActivity(id) {
    return apiClient.delete(`/activities/${id}`);
  },
  
  getDailySummary(date) {
    return apiClient.get(`/activities/daily-summary/${date}`);
  },
  
  getStatistics() {
    // В вашем API нет отдельного метода для статистики
    // Можно получить все активности и посчитать
    return this.getActivities();
  }
};