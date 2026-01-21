<template>
    <div class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5)">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <i class="bi me-2" :class="isEditing ? 'bi-pencil-square' : 'bi-plus-circle'"></i>
                        {{ isEditing ? 'Редактировать активность' : 'Новая активность' }}
                    </h5>
                    <button type="button" class="btn-close" @click="$emit('cancel')"></button>
                </div>

                <form @submit.prevent="handleSubmit">
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="activityDate" class="form-label">Дата активности *</label>
                                <input type="date"
                                       class="form-control"
                                       id="activityDate"
                                       v-model="formData.activityDate"
                                       required
                                       :max="today">
                            </div>

                            <div class="col-md-6 mb-3">
                                <label for="activityMinutes" class="form-label">Продолжительность (минут) *</label>
                                <input type="number"
                                       class="form-control"
                                       id="activityMinutes"
                                       v-model="formData.minutes"
                                       required
                                       min="1"
                                       max="1440"
                                       step="1">
                                <small class="form-text text-muted">
                                    Максимум 1440 минут в сутки
                                </small>
                            </div>
                        </div>

                        <div class="mb-3">
                            <label for="activityExercise" class="form-label">Упражнение *</label>
                            <select class="form-select"
                                    id="activityExercise"
                                    v-model="formData.exerciseId"
                                    required
                                    :disabled="isExerciseDisabled">
                                <option value="">Выберите упражнение...</option>
                                <option v-for="exercise in activeExercises"
                                        :key="exercise.id"
                                        :value="exercise.id">
                                    {{ exercise.name }} ({{ exercise.trainingProgramName }})
                                </option>
                            </select>
                            <small v-if="isExerciseDisabled" class="text-warning">
                                 Упражнение стало неактивным, изменение запрещено
                            </small>
                        </div>

                        <div class="mb-3">
                            <label for="activityNotes" class="form-label">Примечания</label>
                            <textarea class="form-control"
                                      id="activityNotes"
                                      v-model="formData.notes"
                                      rows="3"
                                      maxlength="500"
                                      placeholder="Например: Самочувствие хорошее, вес 50кг"></textarea>
                            <small class="form-text text-muted">
                                {{ formData.notes?.length || 0 }}/500 символов
                            </small>
                        </div>

                        <!-- ���������� � ������� ������ -->
                        <div v-if="dailyLimitWarning" class="alert alert-warning">
                            <i class="bi bi-exclamation-triangle me-2"></i>
                            {{ dailyLimitWarning }}
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="$emit('cancel')">
                            Отмена
                        </button>
                        <button type="submit"
                                class="btn btn-primary"
                                :disabled="isSubmitting">
                            <span v-if="isSubmitting" class="spinner-border spinner-border-sm me-2"></span>
                            {{ isEditing ? 'Сохранить' : 'Создать' }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { format } from 'date-fns'
import api from '@/services/api'

const props = defineProps({
  activity: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['save', 'cancel'])

const formData = ref({
  activityDate: format(new Date(), 'yyyy-MM-dd'),
  minutes: 30,
  exerciseId: '',
  notes: ''
})

const activeExercises = ref([])
const dailyTotalMinutes = ref(0)
const isSubmitting = ref(false)
const isExerciseDisabled = ref(false)

const today = computed(() => format(new Date(), 'yyyy-MM-dd'))

const isEditing = computed(() => !!props.program)

const dailyLimitWarning = computed(() => {
  const total = dailyTotalMinutes.value + (parseInt(formData.value.minutes) || 0)
  if (total > 1440) {
    return `Превышен дневной лимит! Доступно: ${1440 - dailyTotalMinutes.value} минут`
  }
  if (total > 1000) {
    return `Близко к лимиту! Доступно: ${1440 - dailyTotalMinutes.value} минут`
  }
  return null
})

onMounted(async () => {
  await loadActiveExercises()

  if (props.activity) {
    // ��� ��������������
    formData.value = {
      activityDate: format(new Date(props.activity.activityDate), 'yyyy-MM-dd'),
      minutes: props.activity.minutes,
      exerciseId: props.activity.exerciseId,
      notes: props.activity.notes || ''
    }

    // ���������, ������� �� ����������
    const exercise = activeExercises.value.find(e => e.id === props.activity.exerciseId)
    isExerciseDisabled.value = !exercise
  }

  await loadDailyTotal()
})

// ��������� ������� ����� ��� ��������� ����
watch(() => formData.value.activityDate, async () => {
  await loadDailyTotal()
})

async function loadActiveExercises() {
  try {
    activeExercises.value = await api.getActiveExercises()

    // ���� �� �����������, �������� ������ ���������� �� ���������
    if (!props.activity && activeExercises.value.length > 0) {
      formData.value.exerciseId = activeExercises.value[0].id
    }
  } catch (error) {
    console.error('Error loading exercises:', error)
  }
}

async function loadDailyTotal() {
  try {
    const dateStr = formData.value.activityDate
    const summary = await api.getActivitiesByDay(dateStr)
    dailyTotalMinutes.value = summary.totalMinutes

    // ���� �����������, �������� ������� ����������
    if (props.activity) {
      dailyTotalMinutes.value -= props.activity.minutes
    }
  } catch (error) {
    console.error('Error loading daily total:', error)
    dailyTotalMinutes.value = 0
  }
}

async function handleSubmit() {
  if (!validateForm()) return

  isSubmitting.value = true

  try {
    const activityData = {
      activityDate: formData.value.activityDate,
      minutes: parseInt(formData.value.minutes),
      exerciseId: parseInt(formData.value.exerciseId),
      notes: formData.value.notes?.trim() || null
    }

    emit('save', activityData)
  } catch (error) {
    console.error('Error in form submission:', error)
  } finally {
    isSubmitting.value = false
  }
}

function validateForm() {
  if (!formData.value.activityDate) {
    alert('Пожалуйста, выберите дату')
    return false
  }

  if (!formData.value.minutes || formData.value.minutes < 1 || formData.value.minutes > 1440) {
    alert('Продолжительность должна быть от 1 до 1440 минут')
    return false
  }

  if (!formData.value.exerciseId) {
    alert('Пожалуйста, выберите упражнение')
    return false
  }

  const total = dailyTotalMinutes.value + parseInt(formData.value.minutes)
  if (total > 1440) {
    alert(`Превышен дневной лимит времени! Доступно: ${1440 - dailyTotalMinutes.value} минут`)
    return false
  }

  return true
}
</script>

<style scoped>
    .modal {
        backdrop-filter: blur(2px);
    }

    .form-label {
        font-weight: 500;
    }

    .alert {
        margin-bottom: 0;
    }

    textarea {
        resize: vertical;
    }
</style>