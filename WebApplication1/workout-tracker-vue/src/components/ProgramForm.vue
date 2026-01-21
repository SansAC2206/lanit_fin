<template>
    <div class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5)">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <i class="bi me-2" :class="isEditing ? 'bi-pencil-square' : 'bi-plus-circle'"></i>
                        {{ isEditing ? 'Редактировать программу' : 'Новая программа' }}
                    </h5>
                    <button type="button" class="btn-close" @click="$emit('cancel')"></button>
                </div>

                <form @submit.prevent="handleSubmit">
                    <div class="modal-body">
                        <div class="mb-3">
                            <label for="programName" class="form-label">Название программы *</label>
                            <input type="text"
                                   class="form-control"
                                   id="programName"
                                   v-model="formData.name"
                                   required
                                   maxlength="100">
                        </div>

                        <div class="mb-3">
                            <label for="programType" class="form-label">Тип программы *</label>
                            <select class="form-select"
                                    id="programType"
                                    v-model="formData.type"
                                    required>
                                <option value="">Выберите тип...</option>
                                <option value="Силовая">Силовая</option>
                                <option value="Кардио">Кардио</option>
                                <option value="Растяжка">Растяжка</option>
                                <option value="Функциональная">Функциональная</option>
                                <option value="Йога">Йога</option>
                                <option value="Пилатес">Пилатес</option>
                                <option value="Кроссфит">Кроссфит</option>
                            </select>
                        </div>

                        <div class="form-check form-switch mb-3">
                            <input class="form-check-input"
                                   type="checkbox"
                                   id="programActive"
                                   v-model="formData.isActive">
                            <label class="form-check-label" for="programActive">
                                Программа активна
                            </label>
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="$emit('cancel')">
                            Отмена
                        </button>
                        <button type="submit" class="btn btn-primary">
                            {{ isEditing ? 'Сохранить' : 'Создать' }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const props = defineProps({
  program: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['save', 'cancel'])

const formData = ref({
  name: '',
  type: '',
  isActive: true
})

const isEditing = computed(() => !!props.program)

onMounted(() => {
  if (props.program) {
    formData.value = { ...props.program }
  }
})

function handleSubmit() {
  if (!formData.value.name.trim() || !formData.value.type.trim()) {
    alert('Пожалуйста, заполните все обязательные поля')
    return
  }

  emit('save', {
    name: formData.value.name.trim(),
    type: formData.value.type.trim(),
    isActive: formData.value.isActive
  })
}
</script>

<style scoped>
    .modal {
        backdrop-filter: blur(2px);
    }

    .form-label {
        font-weight: 500;
    }

    .form-check-input:checked {
        background-color: #0d6efd;
        border-color: #0d6efd;
    }
</style>