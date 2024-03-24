<template>
  <form @submit.prevent="addUser">
    <v-card-text class="px-8 py-4">
      <div class="text-body-1">
        <v-row>
          <v-col cols="6" class="pb-0">
            <v-label class="text-caption">Full Name</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="addUserInfo.fullName.value"
              :error-messages="addUserInfo.fullName.errorMessage"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="6" class="pb-0">
            <v-label class="text-caption">Email</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="addUserInfo.email.value"
              :error-messages="addUserInfo.email.errorMessage"
              hide-details="auto"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <v-label class="text-caption">Role</v-label>
            <v-select
              :items="roleList"
              item-title="name"
              item-value="name"
              placeholder="Select role"
              density="compact"
              variant="outlined"
              v-model="addUserInfo.role.value"
              :error-messages="addUserInfo.role.errorMessage"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="4">
            <v-label class="text-caption">Username</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="addUserInfo.username.value"
              :error-messages="addUserInfo.username.errorMessage"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="4">
            <v-label class="text-caption">Password</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="addUserInfo.password.value"
              :error-messages="addUserInfo.password.errorMessage"
              hide-details="auto"
            />
          </v-col>
        </v-row>
      </div>
    </v-card-text>
    <v-card-actions class="p-3 justify-content-end">  
      <v-btn color="primary" type="submit">Submit</v-btn>
    </v-card-actions>
  </form>
</template>

<script setup>
import { useField, useForm } from 'vee-validate'

// Properties, Emit & Model
const emit = defineEmits(['close-modal'])

// Data
const { handleSubmit } = useForm({
  validationSchema: {
    fullName(value) {
      return value ? true : 'Full name is required'
    },
    username(value) {
      return value ? true : 'Username is required'
    },
    email(value) {
      return value ? true : 'Email is required'
    },
    role(value) {
      return value ? true : 'User role is required'
    },
    password(value) {
      return value ? true : 'Password is required'
    }
  }
})
const addUserInfo = ref({
  fullName: useField('fullName'),
  username: useField('username'),
  email: useField('email'),
  role: useField('role'),
  password: useField('password'),
})
const { data: roleList } = await useFetchCustom.$get("/User/RoleList")

// Methods
const addUser = handleSubmit(async(values) => {
  try {
    const result = await useFetchCustom.$post("/User", values)
    if (!result.error) {
      emit('close-modal', false)
      addUserInfo.value.fullName.resetField()
      addUserInfo.value.username.resetField()
      addUserInfo.value.email.resetField()
      addUserInfo.value.role.resetField()
      addUserInfo.value.password.resetField()
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
})
</script>