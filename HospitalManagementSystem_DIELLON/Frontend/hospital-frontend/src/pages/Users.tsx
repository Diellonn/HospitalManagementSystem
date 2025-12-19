import { useEffect, useState } from 'react'
import { usersApi } from '../services/api'
import type { User } from '../types'
import './Common.css'

export default function Users() {
  const [users, setUsers] = useState<User[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const usersData = await usersApi.getAll()
      setUsers(usersData)
    } catch (error) {
      console.error('Error loading users:', error)
    } finally {
      setLoading(false)
    }
  }

  if (loading) return <div>Loading...</div>

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Përdoruesit</h1>
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Username</th>
            <th>Email</th>
            <th>Roli</th>
            <th>Statusi</th>
            <th>Data e Krijimit</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.userId}>
              <td>{user.userId}</td>
              <td>{user.username}</td>
              <td>{user.email}</td>
              <td>
                <span className={`status-badge status-${user.role?.toLowerCase() || 'default'}`}>
                  {user.role}
                </span>
              </td>
              <td>
                <span className={`status-badge ${user.isActive ? 'status-completed' : 'status-cancelled'}`}>
                  {user.isActive ? 'Aktiv' : 'Jo Aktiv'}
                </span>
              </td>
              <td>{user.createdAt ? new Date(user.createdAt).toLocaleDateString() : '-'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

