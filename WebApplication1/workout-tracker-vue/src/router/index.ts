import { createRouter, createWebHistory } from 'vue-router';
import Dashboard from '@/views/DashboardView.vue';
import Programs from '@/views/ProgramsView.vue';
import Exercises from '@/views/ExercisesView.vue';
import Activities from '@/views/ActivitiesView.vue';
import Calendar from '@/views/Calendar.vue';

const routes = [
  {
    path: '/',
    name: 'Dashboard',
    component: Dashboard,
  },
  {
    path: '/programs',
    name: 'Programs',
    component: Programs,
  },
  {
    path: '/exercises',
    name: 'Exercises',
    component: Exercises,
  },
  {
    path: '/activities',
    name: 'Activities',
    component: Activities,
  },
  {
    path: '/calendar',
    name: 'Calendar',
    component: Calendar,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;