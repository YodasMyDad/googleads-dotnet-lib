' Copyright 2011, Google Inc. All Rights Reserved.
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.

' Author: api.anash@gmail.com (Anash P. Oommen)

Imports Google.Api.Ads.AdWords.Lib
Imports Google.Api.Ads.AdWords.v201109

Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace Google.Api.Ads.AdWords.Examples.VB.v201109
  ''' <summary>
  ''' This code example retrieves all text ads given an existing ad group. To
  ''' add ads to an existing ad group, run AddTextAds.vb.
  '''
  ''' Tags: AdGroupAdService.get
  ''' </summary>
  Public Class GetTextAds
    Inherits ExampleBase
    ''' <summary>
    ''' Main method, to run this code example as a standalone application.
    ''' </summary>
    ''' <param name="args">The command line arguments.</param>
    Public Shared Sub Main(ByVal args As String())
      Dim codeExample As ExampleBase = New GetTextAds
      Console.WriteLine(codeExample.Description)
      Try
        codeExample.Run(New AdWordsUser, codeExample.GetParameters, Console.Out)
      Catch ex As Exception
        Console.WriteLine("An exception occurred while running this code example. {0}", _
            ExampleUtilities.FormatException(ex))
      End Try
    End Sub

    ''' <summary>
    ''' Returns a description about the code example.
    ''' </summary>
    Public Overrides ReadOnly Property Description() As String
      Get
        Return "This code example retrieves all text ads given an existing ad group. To add " & _
            "text ads to an existing ad group, run AddTextAds.vb."
      End Get
    End Property

    ''' <summary>
    ''' Gets the list of parameter names required to run this code example.
    ''' </summary>
    ''' <returns>
    ''' A list of parameter names for this code example.
    ''' </returns>
    Public Overrides Function GetParameterNames() As String()
      Return New String() {"ADGROUP_ID"}
    End Function

    ''' <summary>
    ''' Runs the code example.
    ''' </summary>
    ''' <param name="user">The AdWords user.</param>
    ''' <param name="parameters">The parameters for running the code
    ''' example.</param>
    ''' <param name="writer">The stream writer to which script output should be
    ''' written.</param>
    Public Overrides Sub Run(ByVal user As AdWordsUser, ByVal parameters As  _
        Dictionary(Of String, String), ByVal writer As TextWriter)
      ' Get the AdGroupAdService.
      Dim service As AdGroupAdService = user.GetService(AdWordsService.v201109.AdGroupAdService)

      Dim adGroupId As Long = Long.Parse(parameters("ADGROUP_ID"))

      ' Create a selector.
      Dim selector As New Selector
      selector.fields = New String() {"Id", "Status", "Headline", "Description1", _
          "Description2", "DisplayUrl"}

      ' Set the sort order.
      Dim orderBy As New OrderBy
      orderBy.field = "Id"
      orderBy.sortOrder = SortOrder.ASCENDING
      selector.ordering = New OrderBy() {orderBy}

      ' Restrict the fetch to only the selected ad group id.
      Dim adGroupPredicate As New Predicate
      adGroupPredicate.field = "AdGroupId"
      adGroupPredicate.operator = PredicateOperator.EQUALS
      adGroupPredicate.values = New String() {adGroupId.ToString}

      ' Retrieve only text ads.
      Dim typePredicate As New Predicate
      typePredicate.field = "AdType"
      typePredicate.operator = PredicateOperator.EQUALS
      typePredicate.values = New String() {"TEXT_AD"}

      ' By default disabled ads aren't returned by the selector. To return them
      ' include the DISABLED status in the statuses field.
      Dim statusPredicate As New Predicate
      statusPredicate.field = "Status"
      statusPredicate.operator = PredicateOperator.IN

      statusPredicate.values = New String() {AdGroupAdStatus.ENABLED.ToString, _
          AdGroupAdStatus.PAUSED.ToString, AdGroupAdStatus.DISABLED.ToString}

      selector.predicates = New Predicate() {adGroupPredicate, statusPredicate, typePredicate}

      ' Select the selector paging.
      selector.paging = New Paging

      Dim offset As Integer = 0
      Dim pageSize As Integer = 500

      Dim page As New AdGroupAdPage

      Try
        Do
          selector.paging.startIndex = offset
          selector.paging.numberResults = pageSize

          ' Get the text ads.
          page = service.get(selector)

          ' Display the results.
          If ((Not page Is Nothing) AndAlso (Not page.entries Is Nothing)) Then
            Dim i As Integer = offset

            For Each adGroupAd As AdGroupAd In page.entries
              Dim textAd As TextAd = adGroupAd.ad
              writer.WriteLine("{0}) Ad id is {1} and status is {2}", i, textAd.id, _
                  adGroupAd.status)
              writer.WriteLine("  {0}\n  {1}\n  {2}\n  {3}", textAd.headline, _
                  textAd.description1, textAd.description2, textAd.displayUrl)
            Next
            i += 1
          End If
          offset = offset + pageSize
        Loop While (offset < page.totalNumEntries)
        writer.WriteLine("Number of text ads found: {0}", page.totalNumEntries)
      Catch ex As Exception
        Throw New System.ApplicationException("Failed to get text ads.", ex)
      End Try
    End Sub
  End Class
End Namespace
