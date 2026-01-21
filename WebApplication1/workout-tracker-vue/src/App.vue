<template>
  <div id="app">
    <!-- Навигация -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary shadow-sm">
      <div class="container">
        <router-link class="navbar-brand d-flex align-items-center" to="/">
          <i class="bi bi-activity me-2"></i>
          <span>Workout Tracker</span>
        </router-link>
        
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto">
            <li class="nav-item">
              <router-link class="nav-link" to="/">
                <i class="bi bi-speedometer2 me-1"></i>Dashboard
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/programs">
                <i class="bi bi-list-task me-1"></i>Programs
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/exercises">
                <i class="bi bi-bicycle me-1"></i>Exercises
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/activities">
                <i class="bi bi-clock-history me-1"></i>Activities
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/calendar">
                <i class="bi bi-calendar3 me-1"></i>Calendar
              </router-link>
            </li>
          </ul>
        </div>
      </div>
    </nav>
    
    <!-- Основной контент -->
    <main class="py-4">
      <div class="container">
        <router-view />
      </div>
    </main>
    
    <!-- Футер -->
    <footer class="bg-light border-top py-3 mt-5">
      <div class="container text-center">
        <small class="text-muted">
          Workout Tracker v1.0 | API: http://localhost:5000
        </small>
      </div>
    </footer>
  </div>
</template>

<script>
export default {
  name: 'App',
};
</script>

<style>
.nav-link.router-link-active {
  background-color: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
}
</style>