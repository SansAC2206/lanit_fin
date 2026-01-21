<template>
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary shadow">
        <div class="container-fluid">
            <router-link class="navbar-brand" to="/">
                <i class="bi bi-activity me-2"></i>
                Workout Tracker
            </router-link>

            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>

            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item">
                        <router-link class="nav-link" to="/">
                            <i class="bi bi-speedometer2 me-1"></i> Дашборд
                        </router-link>
                    </li>
                    <li class="nav-item">
                        <router-link class="nav-link" to="/programs">
                            <i class="bi bi-list-task me-1"></i> Программы
                        </router-link>
                    </li>
                    <li class="nav-item">
                        <router-link class="nav-link" to="/exercises">
                            <i class="bi bi-bicycle me-1"></i> Упражнения
                        </router-link>
                    </li>
                    <li class="nav-item">
                        <router-link class="nav-link" to="/activities">
                            <i class="bi bi-clock-history me-1"></i> Активности
                        </router-link>
                    </li>
                    <li class="nav-item">
                        <router-link class="nav-link" to="/calendar">
                            <i class="bi bi-calendar3 me-1"></i> Календарь
                        </router-link>
                    </li>
                </ul>

                <div class="d-flex">
                    <div class="dropdown">
                        <button class="btn btn-outline-light dropdown-toggle" type="button" data-bs-toggle="dropdown">
                            <i class="bi bi-person-circle me-1"></i> Пользователь
                        </button>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item" href="#"><i class="bi bi-gear me-2"></i>Настройки</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li><a class="dropdown-item" href="#"><i class="bi bi-box-arrow-right me-2"></i>Выход</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </nav>
</template>

<script setup>
// Navigation component logic
</script>

<style scoped>
    .navbar-brand {
        font-weight: 600;
        font-size: 1.3rem;
    }

    .nav-link {
        font-weight: 500;
    }

        .nav-link.router-link-active {
            color: #fff !important;
            background-color: rgba(255, 255, 255, 0.1);
            border-radius: 4px;
        }
</style>