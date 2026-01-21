import { format, startOfMonth, endOfMonth, eachDayOfInterval, isSameDay } from 'date-fns'
import api from './api'

export default {
    // Вспомогательные методы для работы с активностями
    getActivityLevel(minutes) {
        if (minutes < 30) return { level: 'low', color: 'warning', label: 'Низкая' }
        if (minutes < 90) return { level: 'normal', color: 'success', label: 'Нормальная' }
        return { level: 'high', color: 'danger', label: 'Высокая' }
    },

    // Генерация календаря с данными
    async generateCalendar(year, month) {
        const monthStart = startOfMonth(new Date(year, month - 1))
        const monthEnd = endOfMonth(monthStart)

        const { dailySummaries } = await api.getActivitiesByMonth(year, month)

        const days = eachDayOfInterval({ start: monthStart, end: monthEnd })

        return days.map(day => {
            const summary = dailySummaries.find(s =>
                isSameDay(new Date(s.date), day)
            )

            return {
                date: day,
                dayNumber: format(day, 'd'),
                isCurrentMonth: true,
                totalMinutes: summary?.totalMinutes || 0,
                activityLevel: summary?.activityLevel || 'none',
                activities: summary?.activities || []
            }
        })
    },

    // Форматирование времени
    formatMinutes(minutes) {
        const hours = Math.floor(minutes / 60)
        const mins = minutes % 60

        if (hours === 0) return `${mins} мин`
        if (mins === 0) return `${hours} ч`
        return `${hours} ч ${mins} мин`
    },

    // Получение статистики за период
    async getPeriodStatistics(startDate, endDate) {
        const stats = await api.getStatistics(startDate, endDate)

        return {
            ...stats,
            averageDailyMinutes: Math.round(stats.averageDailyMinutes || 0),
            formattedTotal: this.formatMinutes(stats.totalMinutes || 0)
        }
    }
}