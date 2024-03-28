<template>
  <form @submit.prevent="editUser">
    <v-card-text class="px-8 py-4">
      <div class="text-body-1">
        <v-row>
          <v-col class="pb-0">
            <v-label class="text-caption">Full Name</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="editUserInfo.fullName.value"
              :error-messages="editUserInfo.fullName.errorMessage"
              hide-details="auto"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col class="pb-0">
            <v-label class="text-caption">Username</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="editUserInfo.username.value"
              :error-messages="editUserInfo.username.errorMessage"
              hide-details="auto"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col class="pb-0">
            <v-label class="text-caption">Email</v-label>
            <v-text-field
              variant="outlined"
              density="compact"
              v-model="editUserInfo.email.value"
              :error-messages="editUserInfo.email.errorMessage"
              hide-details="auto"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-label class="text-caption">Role</v-label>
            <v-select
              :items="roleList"
              item-title="name"
              item-value="name"
              placeholder="Select role"
              density="compact"
              variant="outlined"
              v-model="editUserInfo.role.value"
              :error-messages="editUserInfo.role.errorMessage"
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
const props = defineProps({
  userInfo: Object,
})
const emit = defineEmits(['close-modal'])

// Data
const { handleSubmit } = useForm({
  initialValues: {
    fullName: props.userInfo.fullName,
    username: props.userInfo.username,
    email: props.userInfo.email,
    role: props.userInfo.role,
  },
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
    }
  }
})
const editUserInfo = ref({
  fullName: useField('fullName'),
  username: useField('username'),
  email: useField('email'),
  role: useField('role'),
})
const { data: roleList } = await useFetchCustom.$get("/User/RoleList")

// Methods
const editUser = handleSubmit(async(values) => {
  try {
    const result = await useFetchCustom.$put(`/User/${props.userInfo.id}`, values)
    if (!result.error) {
      emit('close-modal', false)
      editUserInfo.value.fullName.resetField()
      editUserInfo.value.username.resetField()
      editUserInfo.value.email.resetField()
      editUserInfo.value.role.resetField()
      ElNotification.success({ message: result.message })
      refreshNuxtData()
    }
    else {
      ElNotification.error({ message: result.message })
    }
  } catch { ElNotification.error({ message: "There is a problem with the server. Please try again later." }) }
})
</script>