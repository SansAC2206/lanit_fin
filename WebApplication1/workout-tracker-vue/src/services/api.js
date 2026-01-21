import axios from 'axios'

// Базовый URL для API
const API_BASE_URL = 'http://localhost:5000/api'

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    }
})

// Обработчик ошибок
api.interceptors.response.use(
    response => response.data,
    error => {
        console.error('API Error:', error.message)
        if (error.response) {
            console.error('Response data:', error.response.data)
            console.error('Response status:', error.response.status)
        }
        throw error
    }
)

export default {
    // Training Programs
    getPrograms() {
        return api.get('/trainingprograms')
    },

    createProgram(program) {
        return api.post('/trainingprograms', program)
    },

    updateProgram(id, program) {
        return api.put(`/trainingprograms/${id}`, program)
    },

    deleteProgram(id) {
        return api.delete(`/trainingprograms/${id}`)
    },

    // Exercises
    getExercises() {
        return api.get('/exercises')
    },

    getActiveExercises() {
        return api.get('/exercises/active')
    },

    createExercise(exercise) {
        return api.post('/exercises', exercise)
    },

    // Workout Activities
    getActivities() {
        return api.get('/workoutactivities')
    },

    createActivity(activity) {
        return api.post('/workoutactivities', activity)
    },

    getStatistics() {
        return api.get('/workoutactivities/statistics')
    }
}