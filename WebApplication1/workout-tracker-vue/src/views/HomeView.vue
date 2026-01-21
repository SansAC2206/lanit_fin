<template>
    <div class="container py-4">
        <div class="text-center mb-5">
            <h1 class="display-4 text-primary mb-3">
                <i class="bi bi-activity me-3"></i>Workout Tracker
            </h1>
            <p class="lead text-muted">
                Система учета тренировок и физической активности
            </p>
        </div>

        <div class="row mb-5">
            <div class="col-md-4 mb-4">
                <div class="card h-100 shadow border-0">
                    <div class="card-body text-center">
                        <i class="bi bi-list-task display-4 text-primary mb-3"></i>
                        <h5 class="card-title">Программы тренировок</h5>
                        <p class="card-text text-muted">
                            Создавайте и управляйте тренировочными программами
                        </p>
                        <router-link to="/programs" class="btn btn-outline-primary">
                            Перейти
                        </router-link>
                    </div>
                </div>
            </div>

            <div class="col-md-4 mb-4">
                <div class="card h-100 shadow border-0">
                    <div class="card-body text-center">
                        <i class="bi bi-bicycle display-4 text-success mb-3"></i>
                        <h5 class="card-title">Упражнения</h5>
                        <p class="card-text text-muted">
                            Управляйте списком упражнений для каждой программы
                        </p>
                        <router-link to="/exercises" class="btn btn-outline-success">
                            Перейти
                        </router-link>
                    </div>
                </div>
            </div>

            <div class="col-md-4 mb-4">
                <div class="card h-100 shadow border-0">
                    <div class="card-body text-center">
                        <i class="bi bi-clock-history display-4 text-warning mb-3"></i>
                        <h5 class="card-title">Активности</h5>
                        <p class="card-text text-muted">
                           Отслеживайте выполненные тренировки
                        </p>
                        <router-link to="/activities" class="btn btn-outline-warning">
                            Перейти
                        </router-link>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-6 mb-4">
                <div class="card shadow border-0">
                    <div class="card-body">
                        <h5 class="card-title">
                            <i class="bi bi-calendar3 text-info me-2"></i>Календарь тренировок
                        </h5>
                        <p class="card-text">
                            Просматривайте свои активности в календарном виде с цветовой индикацией
                        </p>
                        <router-link to="/calendar" class="btn btn-outline-info">
                            <i class="bi bi-arrow-right"></i> Открыть календарь
                        </router-link>
                    </div>
                </div>
            </div>

            <div class="col-md-6 mb-4">
                <div class="card shadow border-0">
                    <div class="card-body">
                        <h5 class="card-title">
                            <i class="bi bi-graph-up text-danger me-2"></i>Статистика
                        </h5>
                        <p class="card-text">
                            Анализируйте свою активность и прогресс
                        </p>
                        <button @click="testApi" class="btn btn-outline-danger">
                            <span v-if="testing" class="spinner-border spinner-border-sm me-2"></span>
                            <i v-else class="bi bi-plug me-2"></i>
                            {{ testing ? 'Проверка...' : 'Проверить API' }}
                        </button>
                        <div v-if="apiStatus" class="mt-2">
                            <small :class="apiStatus.success ? 'text-success' : 'text-danger'">
                                <i :class="apiStatus.success ? 'bi bi-check-circle' : 'bi bi-x-circle'"></i>
                                {{ apiStatus.message }}
                            </small>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="mt-5 pt-4 border-top text-center">
            <p class="text-muted small">
                Workout Tracker v1.0 | API: <code>http://localhost:5000</code>
            </p>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue'
import api from '@/services/api'

const testing = ref(false)
const apiStatus = ref(null)

async function testApi() {
  testing.value = true
  apiStatus.value = null

  try {
    // ������� �������� ���������
    const programs = await api.getPrograms()
    apiStatus.value = {
      success: true,
      message: `API работает! Найдено программ: ${programs.length}`
    }
  } catch (error) {
    apiStatus.value = {
      success: false,
      message: `Ошибка подключения к API: ${error.message}`
    }
  } finally {
    testing.value = false
  }
}
</script>

<style scoped>
    .card {
        transition: transform 0.2s;
    }

        .card:hover {
            transform: translateY(-5px);
        }
</style>