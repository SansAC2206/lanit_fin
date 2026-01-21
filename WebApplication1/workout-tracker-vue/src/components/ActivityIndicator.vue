<template>
    <div class="activity-indicator"
         :class="indicatorClass"
         :style="indicatorStyle"
         :title="tooltip">
        <i v-if="showIcon" :class="iconClass"></i>
        <span v-if="showMinutes">{{ minutes }}�</span>
    </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  minutes: {
    type: Number,
    default: 0
  },
  size: {
    type: String,
    default: 'md', // sm, md, lg
    validator: (value) => ['sm', 'md', 'lg'].includes(value)
  },
  showMinutes: {
    type: Boolean,
    default: true
  },
  showIcon: {
    type: Boolean,
    default: false
  }
})

const activityLevel = computed(() => {
  const mins = props.minutes
  if (mins < 30) return 'low'
  if (mins < 90) return 'normal'
  return 'high'
})

const indicatorClass = computed(() => {
  const classes = ['activity-indicator']
  classes.push(`activity-${activityLevel.value}`)
  classes.push(`size-${props.size}`)
  return classes.join(' ')
})

const indicatorStyle = computed(() => {
  const styles = {}

  if (props.size === 'sm') {
    styles.width = '24px'
    styles.height = '24px'
    styles.fontSize = '10px'
  } else if (props.size === 'md') {
    styles.width = '40px'
    styles.height = '40px'
    styles.fontSize = '12px'
  } else if (props.size === 'lg') {
    styles.width = '60px'
    styles.height = '60px'
    styles.fontSize = '14px'
  }

  return styles
})

const tooltip = computed(() => {
  const level = activityLevel.value
  if (level === 'low') return 'Низкая активность'
  if (level === 'normal') return 'Нормальная активность'
  return 'Высокая активность'
})

const iconClass = computed(() => {
  const level = activityLevel.value
  if (level === 'low') return 'bi bi-emoji-neutral'
  if (level === 'normal') return 'bi bi-emoji-smile'
  return 'bi bi-emoji-dizzy'
})
</script>

<style scoped>
    .activity-indicator {
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: 600;
        color: white;
        box-shadow: 0 2px 4px rgba(0,0,0,0.2);
        transition: all 0.3s ease;
    }

        .activity-indicator:hover {
            transform: scale(1.1);
            box-shadow: 0 4px 8px rgba(0,0,0,0.3);
        }

    .activity-low {
        background-color: #ffc107;
        border: 2px solid #ffc107;
    }

    .activity-normal {
        background-color: #198754;
        border: 2px solid #198754;
    }

    .activity-high {
        background-color: #dc3545;
        border: 2px solid #dc3545;
    }

    .size-sm {
        width: 24px;
        height: 24px;
        font-size: 10px;
    }

    .size-md {
        width: 40px;
        height: 40px;
        font-size: 12px;
    }

    .size-lg {
        width: 60px;
        height: 60px;
        font-size: 14px;
    }
</style>