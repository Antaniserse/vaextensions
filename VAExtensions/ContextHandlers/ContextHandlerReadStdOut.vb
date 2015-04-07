﻿Public Class ContextHandlerReadStdOut
   Inherits ContextHandlerBase

   Public Sub New(ByVal context As ContextFactory.Contexts, ByRef state As Dictionary(Of String, Object), ByRef conditions As Dictionary(Of String, Nullable(Of Int16)), ByRef textValues As Dictionary(Of String, String), ByRef extendedValues As Dictionary(Of String, Object))
      MyBase.New(context, state, conditions, textValues, extendedValues)
   End Sub

   Public Overrides Function Execute() As Boolean
      Dim newFile As DownloadedFile = Nothing
      Dim regexPattern As String

      If Not m_TextValues.ContainsKey(App.KEY_FILE) Then
         m_Conditions(App.KEY_ERROR) = ERR_CONTEXT
         m_TextValues(App.KEY_RESULT) = String.Format("Unknown file name. Text variable ""{0}"" not set.", App.KEY_FILE)
         Return False
      End If

      Try
         Dim pName As String = m_TextValues(App.KEY_FILE)
         Dim pInfo As ProcessStartInfo
         Dim pOutput As String
         Dim p As Process

         If Not IO.Path.IsPathRooted(pName) Then
            pName = IO.Path.Combine(IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly.Location), pName)
         End If
         pInfo = New ProcessStartInfo(pName) With {.UseShellExecute = False, .CreateNoWindow = True, .RedirectStandardOutput = True}
         If Not String.IsNullOrEmpty(m_TextValues(App.KEY_ARGUMENTS)) Then
            pInfo.Arguments = m_TextValues(App.KEY_ARGUMENTS)
         End If

         p = Process.Start(pInfo)
         pOutput = p.StandardOutput.ReadToEnd
         p.WaitForExit()

         m_TextValues(App.KEY_RESULT) = App.LimitResponse(pOutput)
      Catch ex As Exception
         m_Conditions(App.KEY_ERROR) = ERR_IO
         m_TextValues(App.KEY_RESULT) = String.Format("Error running process ""{0}"" - {1}.", m_TextValues(App.KEY_FILE), ex.Message)
         Return False
      End Try

      Return True
   End Function
End Class